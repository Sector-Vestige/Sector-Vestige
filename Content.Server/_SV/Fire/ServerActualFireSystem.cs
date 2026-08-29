using Content.Server.Atmos.EntitySystems;
using Content.Shared._SV.Fire;
using Content.Shared._SV.Utility;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using static Content.Shared.Atmos.Gas;

namespace Content.Server._SV.Fire;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class ServerActualFireSystem : EntitySystem
{
    [Dependency] private AtmosphereSystem _atmosphere = default!;
    [Dependency] private EntityManager _entityManager = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private IRobustRandom _random = default!;
    [Dependency] private EntityLookupSystem _lookupSystem = default!;
    [Dependency] private SharedSolutionContainerSystem _solutionContainerSystem = default!;
    [Dependency] private SharedActualFireSystem _fireSystem = default!;

    private const float ReagentToBurn = 5.0f;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActualFireComponent, ComponentInit>(OnInit);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ActualFireComponent>();
        while (query.MoveNext(out var entity, out var fireComp))
        {
            if (_timing.CurTime < fireComp.TimeTillNextTick)
                continue;
            fireComp.TimeTillNextTick += TimeSpan.FromSeconds(fireComp.TimeBetweenFireTick);

            var tileMixture = _atmosphere.GetTileMixture(entity);

            //if there is an atmosphere, and the temperature of the air is less than the maximum heat of the fire; add heat to it.
            if (tileMixture != null && tileMixture.Temperature < fireComp.MaxFireTemp)
                _atmosphere.AddHeat(tileMixture, fireComp.GenratedHeat);

            if (fireComp.TargetEntity == null)
                continue;

            // get the solution and solution entity from the targeted entity
            if (!_solutionContainerSystem.TryGetSolution(fireComp.TargetEntity.Value,
                    fireComp.TargetEntity.ToString()!,
                    out var solution,
                    out var sol))
                continue;

            //Extinguish the fire if there is nothing to burn, as well as grabbing each flammable reagent from the fire
            if (!_fireSystem.TryGetFlamableReagents(sol, out var flammableReagents) || flammableReagents == null)
            {
                _entityManager.DeleteEntity(entity);
                continue;
            }

            // burn a certain amount of reagents, stored as ReagentsToBurn by getting the ratio that the reagent is, and then multiplying it by how much reagent is being burnt
            foreach (var reagent in flammableReagents)
            {
                var amountToBurn = (reagent.Quantity/flammableReagents.Volume) * ReagentToBurn;
                _solutionContainerSystem.RemoveReagent(solution.Value, reagent.Reagent, amountToBurn);
            }

            // spawn the exhaust gases. This should also handle burning off the oxygen
            if (fireComp.GasSpawnEntries != null)
            {
                foreach (var gas in fireComp.GasSpawnEntries)
                {
                    _atmosphere.AdjustTileMixture(entity, gas.Gas, gas.Amount.Next(_random) / flammableReagents.Volume.Float(), true);
                }
            }
        }
    }

    private void OnInit(EntityUid uid, ActualFireComponent component, ComponentInit args)
    {
        TargetEntity(uid, component);
        _fireSystem.UpdateData(uid);
        Dirty(uid, component);
    }

    public void TryLightFluidFire(EntityUid uid)
    {
        var entity = _entityManager.TryGetComponent<SolutionComponent>(uid, out var solution);

        if (solution == null || solution.Solution.Contents.Count == 0)
            return;

        if (!_fireSystem.CheckFlammability(solution.Solution))
            return;

        //At this point, it should try to light
    }

    /// <summary>
    /// Will try to get a target solution to burn. This will either be provided as either parsing a third target Entity UID as a target, or it will try to find a puddle where the tile the fire is contained in.
    /// It's better to use the specific target function as it's less jank, but it allows admins to spawn the entity wherever and it "just work"
    /// </summary>
    /// <param name="uid">UID of the fire</param>
    /// <param name="component"></param>
    /// <param name="target">Optional target for the fire.</param>
    public void TargetEntity(EntityUid uid, ActualFireComponent component, EntityUid target)
    {
        component.TargetEntity = target;
        return;
    }

    public void TargetEntity(EntityUid uid, ActualFireComponent component)
    {
        var query = _lookupSystem.GetEntitiesIntersecting(uid);
        foreach (var entity in query)
        {
            if (_entityManager.HasComponent<SolutionComponent>(entity))
            {
                component.TargetEntity = entity;
            }
        }
    }
}
