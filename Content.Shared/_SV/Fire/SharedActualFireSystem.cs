using Content.Shared._SV.Utility;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Examine;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.Fire;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class SharedActualFireSystem : EntitySystem
{
    [Dependency] private SharedAtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private EntityManager _entityManager = default!;
    [Dependency] private PrototypeManager _prototypeManager = default!;

    private const float EffectiveOxygenOxidation = 21.8f;
    private const float EffectiveFrezonOxidation = 5.3f;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActualFireComponent, ExaminedEvent>(OnExamine);
    }

    private void OnExamine(EntityUid uid, ActualFireComponent component, ref ExaminedEvent args)
    {
        throw new NotImplementedException();
    }

    public void TryLightFluidFire(EntityUid uid)
    {
        var entity = _entityManager.TryGetComponent<SolutionComponent>(uid, out var solution);

        if (solution == null || solution.Solution.Contents.Count == 0)
            return;

        if (!CheckFlammability(solution.Solution))
            return;

        //At this point, it should try to light
    }

    /// <summary>
    /// Update the component on both the server and client for prediction when the fire is examined
    /// </summary>
    /// <param name="uid">UID of the fire</param>
    public void UpdateData(EntityUid uid)
    {
        if (!_entityManager.TryGetComponent<ActualFireComponent>(uid, out var fire))
            return;

        if (!_entityManager.TryGetComponent<SolutionComponent>(fire.TargetEntity, out var solution) ||
            solution.Solution.Contents.Count == 0)
            return;

        var oxidizer = 0f;
        var fuel = 0f;
        var totalFluid = 0f;
        var generatedHeat = 0f;
        var maxFireHeat = 0f;

        var exhaust = new List<GasSpawnEntry>();

        foreach (var reagent in solution.Solution.Contents)
        {
            if (reagent.Quantity == 0)
                continue;

            var fluid = _prototypeManager.Index<ReagentPrototype>(reagent.Reagent.Prototype);

            if (fluid.FlammableFluid.IsFlammable)
                fuel += reagent.Quantity.Value;

            if (fluid.FlammableFluid.IsOxidizer)
                oxidizer += reagent.Quantity.Value;

            //Generate exhause gas list
            if (fluid.FlammableFluid.ExhaustedGases != null)
                exhaust.AddRange(fluid.FlammableFluid.ExhaustedGases);

            generatedHeat = (fluid.FlammableFluid.GeneratedHeat * reagent.Quantity.Value);
            maxFireHeat = (fluid.FlammableFluid.MaxHeat * reagent.Quantity.Value);
            totalFluid += reagent.Quantity.Value;
        }

        if (fuel == 0)
            return;

        //TODO: FIX THIS
        //This is a shit ass way of representing how oxidized the fire is. I need to somehow be able to use the air as an oxidizer, and have it be a ratio like airOxidizer + (oxidizer / fuel)
        fire.Oxidation = GetOxidation(uid) + oxidizer / fuel / totalFluid;

        //Weird way of calculating this, but this averages out how much heat there is from the fluid that is being burnt, then modify it based on the oxidation (clamped to stop it from getting too stupid)
        fire.GenratedHeat = (generatedHeat / totalFluid) * Math.Clamp(fire.Oxidation, 0f, 25f);
        fire.MaxFireTemp = (maxFireHeat / totalFluid) * Math.Clamp(fire.Oxidation, 0f, 25f);

        fire.GasSpawnEntries = exhaust.ToArray();
        Dirty(uid, fire);
    }

    public bool CheckFlammability(Solution solution)
    {
        foreach (var reagent in solution.Contents)
        {
            if (reagent.Quantity == 0)
                continue;

            var fluid = _prototypeManager.Index<ReagentPrototype>(reagent.Reagent.Prototype);

            if (fluid.FlammableFluid.IsFlammable)
                return true;
        }

        return false;
    }
    public float GetOxidation(EntityUid uid)
    {
        var oxidizer = 0f;
        if(!_atmosphereSystem.TryGetExposedMixture(uid, out var mixture))
            return 0f;

        if (mixture.TotalMoles == 0f || !_atmosphereSystem.IsMixtureOxidizer(mixture))
            return oxidizer;

        //for each oxidizing gas that exists, get the amount that exists in the tile, and then divide it by its EffectiveOxidation coefficient to get how effective the air is at oxidizing.
        //Yes this is overkill for the fact that we only have oxygen as an oxidizing gas, but one can dream.
            foreach (var gas in mixture)
            {
                switch (gas.gas)
                {
                    case Gas.Oxygen:
                        oxidizer += gas.moles / EffectiveOxygenOxidation;
                        break;
                    case Gas.Frezon:
                        oxidizer += gas.moles / EffectiveFrezonOxidation;
                        break;
                }
            }

        return oxidizer;
    }

    public bool TryGetFlamableReagents(Solution solution, out Solution? outSolution)
    {
        var listedSolution = new Solution();
        foreach (var reagent in solution.Contents)
        {
            var fluid = _prototypeManager.Index<ReagentPrototype>(reagent.Reagent.Prototype);

            if (fluid.FlammableFluid.IsFlammable || fluid.FlammableFluid.IsOxidizer)
            {
                listedSolution.AddReagent(reagent);
            }
        }

        if (listedSolution.Contents.Count == 0)
        {
            outSolution = null;
            return false;
        }

        outSolution = listedSolution;
        return true;
    }
}
