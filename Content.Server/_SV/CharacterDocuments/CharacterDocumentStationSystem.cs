using Content.Server._SV.CharacterDocuments.Consoles;
using Content.Shared._SV.CharacterDocuments.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Station;
using Robust.Shared.Prototypes;
using Content.Shared.Station.Systems;

namespace Content.Server._SV.CharacterDocuments;

public sealed partial class CharacterDocumentStationSystem : EntitySystem
{
    [Dependency] private StationSystem _sharedStationSystem = default!;
    [Dependency] private NpcFactionSystem _faction = default!;
    [Dependency] private CharacterDocumentConsoleSystem _documentConsole = default!;

    // Only crew on NanoTrasen's books belong in the roster. A Syndicate operative boarding
    // under cover carries the Syndicate faction instead, so they're filtered out.
    private static readonly ProtoId<NpcFactionPrototype> NanoTrasenFaction = "NanoTrasen";

    public override void Initialize()
    {
        base.Initialize();

        // Membership is registered once documents finish loading (ProfileId known), not at
        // raw spawn — see CharacterDocumentSystem.LoadPlayerDocumentsAsync.
        SubscribeLocalEvent<CharacterDocumentProfileReadyEvent>(OnProfileReady);
        SubscribeLocalEvent<CharacterDocumentComponent, EntParentChangedMessage>(OnParentChanged);
    }

    private void OnProfileReady(CharacterDocumentProfileReadyEvent args)
    {
        if (!TryComp<CharacterDocumentStationComponent>(args.Station, out var stationComp))
            return;

        // Don't list non-NanoTrasen characters (e.g. Syndicate operatives).
        if (!_faction.IsMember(args.Mob, NanoTrasenFaction))
            return;

        // TryAdd so a respawn / clone (same ProfileId) doesn't churn the roster. Entries are
        // never removed for the rest of the round, so the crew member — and their documents —
        // stay accessible even after the body is gibbed.
        if (stationComp.RosterByProfile.TryAdd(args.ProfileId, args.Name))
        {
            Log.Debug($"Added {args.Name} to the roster of {args.Station} it now has a count of {stationComp.RosterByProfile.Count}");
            // Live-update any open consoles so the new crew member shows up without reopening the UI.
            _documentConsole.RefreshAllConsoles();
        }
    }

    public void OnParentChanged(EntityUid uid, CharacterDocumentComponent component, ref EntParentChangedMessage args)
    {
        // ProfileId is 0 until the async document load resolves it; nothing to register yet.
        if (component.ProfileId == 0)
            return;

        // Don't list non-NanoTrasen characters (e.g. Syndicate operatives).
        if (!_faction.IsMember(uid, NanoTrasenFaction))
            return;

        var added = false;
        var stations = _sharedStationSystem.GetStations();
        foreach (EntityUid station in stations)
        {
            if (!TryComp<CharacterDocumentStationComponent>(station, out var stationComp))
                continue;

            if (stationComp.RosterByProfile.TryAdd(component.ProfileId, component.ProfileName))
            {
                Log.Debug($"Added {component.ProfileName} to the roster of {station.Id} it now has a count of {stationComp.RosterByProfile.Count}");
                added = true;
            }
        }

        // Live-update any open consoles so the new crew member shows up without reopening the UI.
        if (added)
            _documentConsole.RefreshAllConsoles();
    }
}
