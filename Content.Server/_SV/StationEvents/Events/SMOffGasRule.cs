using Content.Server._EE.Supermatter.Systems;
using Content.Server._SV.StationEvents.Components;
using Content.Server.Administration.Logs;
using Content.Server.Atmos.EntitySystems;
using Content.Server.GameTicking.Rules;
using Content.Server.StationEvents.Components;
using Content.Shared._EE.Supermatter.Components;
using Content.Shared.Atmos;
using Content.Shared.Database;
using Content.Shared.GameTicking.Components;
using Content.Shared.Station.Components;
using Content.Shared._SV.Utility;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Content.Shared.GameTicking.Rules;
using Content.Shared.Station.Systems;

namespace Content.Server._SV.StationEvents.Events;

/// <summary>
/// This handles...
/// </summary>
public sealed partial class SMOffGasRule : GameRuleSystem<SMOffGasComponent>
{
    [Dependency] private AtmosphereSystem _atmosphere = default!;
    [Dependency] private SupermatterSystem _superMatter = default!;
    [Dependency] private IAdminLogManager _adminLogger = default!;
    [Dependency] private SharedTransformSystem _transform = default!;
    [Dependency] private IEntityManager _entityManager = default!;
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private StationSystem _station = default!;

    private const float LeakCooldown = .25f;

    /// <inheritdoc/>
    protected override void Added(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleAddedEvent args)
    {
        base.Added(uid, component, gameRule, args);
        Log.Level = LogLevel.Debug;
    }

    /// <summary>
    /// SV - Picks the target supermatter. Moved out of Added: ending a rule while it's being added makes
    /// GameTicker.StartGameRule fail, so like upstream rules we only end ourselves once started.
    /// </summary>
    /// <returns>False if the rule ended itself.</returns>
    private bool TrySetup(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule)
    {
        // Upstream removed GameRuleSystem.TryGetRandomStation; StationSystem now provides it.
        if (!_station.TryGetRandomStation<StationEventEligibleComponent>(out var chosenStationEnt))
            return false;

        var chosenStation = chosenStationEnt.Value.Owner;

        //Find a list of all ACTIVE supermatters. Do not select supermatters that are not currently in a nominal status as it could cause some !FUN! problems.
        var possibleTargets = new List<Entity<SupermatterComponent>>();
        var query = EntityQueryEnumerator<SupermatterComponent, TransformComponent>();
        while (query.MoveNext(out var smUid, out var smComponent, out var xform))
        {
            if (smComponent.Status == SupermatterStatusType.Normal && CompOrNull<StationMemberComponent>(xform.GridUid)?.Station == chosenStation)
            {
                possibleTargets.Add((smUid, smComponent));
            }
        }
        //End the event if there is no supermatters available
        if (possibleTargets.Count <= 0)
        {
            Log.Debug($"Terminating event {uid} as no valid supermatter found");
            _adminLogger.Add(LogType.EventStopped, LogImpact.Low, $"Terminating event {uid} as no valid supermatter was found");
            ForceEndSelf(uid, gameRule);
            return false;
        }

        component.Supermatter = RobustRandom.Pick(possibleTargets);

        component.TargetTile = _transform.GetGridOrMapTilePosition(component.Supermatter);
        component.StationUid = chosenStation;
        component.TargetGrid = _transform.GetGrid(component.Supermatter);

        SelectGas(component);
        CheckForValidity(uid, component, gameRule);

        // SV - The old "sv-supermatter-event-added" announcement only ran when the rule was already running during
        // Added, which never happened in practice. Now that this runs from Started it would always fire next to the
        // regular start announcement, so it's left out to keep the old behaviour.
        return _entityManager.HasComponent<SupermatterComponent>(component.Supermatter);
    }

    protected override void Started(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        if (!TrySetup(uid, component, gameRule))
            return;

        BuildAnouncement(component);

        if (!TryComp<StationEventComponent>(uid, out var stationEvent))
            return;

        //build time when the event will end
        if (gameRule.Delay is {} startAfter)
            stationEvent.EndTime = _timing.CurTime + TimeSpan.FromSeconds(component.GasAmount / component.GasRate + startAfter.Next(RobustRandom));
    }

    protected override void ActiveTick(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule, float frameTime)
    {
        base.ActiveTick(uid, component, gameRule, frameTime);

        //count down till next gas leak, do nothing if the cooldown hasn't hit yet, or continue and add more time for the next leak 'event'
        component.TimeTillNextLeak -= frameTime;
        if (component.TimeTillNextLeak > 0f)
            return;
        component.TimeTillNextLeak += LeakCooldown;

        //if by somehow the grid or tile is invalid, or if the atmosphere simulation is disabled, end the event
        CheckForValidity(uid, component, gameRule);

        //stolen from GasLeakRule :)
        var environment = _atmosphere.GetTileMixture(component.TargetGrid, null, component.TargetTile);
        environment?.AdjustMoles(component.SelectedGas, LeakCooldown * component.GasRate);
    }

    protected override void Ended(Entity<SMOffGasComponent> rule, ref GameRuleEndedEvent args)
    {
        base.Ended(rule, ref args);

        if (!_entityManager.TryGetComponent<SupermatterComponent>(rule.Comp.Supermatter, out var supermatter))
            return;

        _superMatter.SendSupermatterAnnouncement(rule.Comp.Supermatter, supermatter, Loc.GetString("sv-supermatter-event-ended"));
    }

    private void SelectGas(SMOffGasComponent component)
    {
        var weightedGasList = new List<GasSpawnEntry>();
        foreach (var gasCollection in component.AllowedGases)
        {
            for (int i = 0; i < gasCollection.Weight; i++)
            {
                weightedGasList.Add(gasCollection);
            }
        }

        var selectedGas = RobustRandom.Pick(weightedGasList);

        //Vary the gas and gas release amount
        component.GasAmount = selectedGas.Amount.Next(RobustRandom);
        component.GasRate = selectedGas.MolPerSecond.Next(RobustRandom);
        component.SelectedGas = selectedGas.Gas;

        Log.Debug($"selected gas is: {selectedGas.Gas}, with amount {component.GasAmount} at rate {component.GasRate}");
    }

    private void BuildAnouncement(SMOffGasComponent component)
    {
        var amountAnnouncement = component.GasAmount switch
        {
            <= 250 => Loc.GetString("sv-off-gas-event-amount-small"),
            <= 500 => Loc.GetString("sv-off-gas-event-amount-medium"),
            <= 750 => Loc.GetString("sv-off-gas-event-amount-large"),
            <= 1000 => Loc.GetString("sv-off-gas-event-amount-excessive"),
            _ => Loc.GetString("sv-off-gas-event-unknown"),
        };

        var rateAnnouncement = component.GasRate switch
        {
            <= 15 => Loc.GetString("sv-off-gas-event-rate-small"),
            <= 25 => Loc.GetString("sv-off-gas-event-rate-medium"),
            <= 35 => Loc.GetString("sv-off-gas-event-rate-large"),
            > 35 => Loc.GetString("sv-off-gas-event-rate-excessive"),
        };

        var gasAnnouncement = component.SelectedGas switch
        {
            Gas.Ammonia => Loc.GetString("gases-ammonia"),
            Gas.CarbonDioxide => Loc.GetString("gases-co2"),
            Gas.Frezon => Loc.GetString("gases-frezon"),
            Gas.Nitrogen => Loc.GetString("gases-nitrogen"),
            Gas.NitrousOxide => Loc.GetString("gases-n2o"),
            Gas.Oxygen => Loc.GetString("gases-oxygen"),
            Gas.WaterVapor => Loc.GetString("gases-water-vapor"),
            Gas.Plasma => Loc.GetString("gases-plasma"),
            Gas.Tritium => Loc.GetString("gases-tritium"),
            _ => Loc.GetString("sv-off-gas-event-unknown"),
        };

        //Build the announcement for the SM to say over engineering comms
        var builtAnouncement = Loc.GetString("sv-off-gas-event-announcement", ("amount", amountAnnouncement), ("rate", rateAnnouncement), ("gas", gasAnnouncement));

        if (!_entityManager.TryGetComponent<SupermatterComponent>(component.Supermatter, out var supermatter))
            return;

        _superMatter.SendSupermatterAnnouncement(component.Supermatter, supermatter, builtAnouncement);
    }

    private void CheckForValidity(EntityUid uid, SMOffGasComponent component, GameRuleComponent gameRule)
    {
        if (component.TargetGrid == null ||
            component.TargetTile == default ||
            Deleted(component.StationUid) ||
            !_atmosphere.IsSimulatedGrid(component.TargetGrid.Value))
        {
            Log.Debug($"SM offgas event {uid} canceled as the location is invalid. Target tile is:  {component.TargetTile}, on grid: {component.TargetGrid} for station ID: {component.StationUid}");
            ForceEndSelf(uid, gameRule);
            if (component.TargetGrid == null)
            {
                Log.Debug("Target grid is null");
                return;
            }
            if (!_atmosphere.IsSimulatedGrid(component.TargetGrid.Value))
                Log.Debug("Target grid is not simulated with atmos");
        }
    }
}
