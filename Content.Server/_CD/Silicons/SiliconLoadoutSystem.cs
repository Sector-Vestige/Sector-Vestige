using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Content.Server.Silicons.Laws;
using Content.Shared.Clothing;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.Silicons.StationAi;
using Robust.Shared.Containers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._CD.Silicons;

public sealed partial class SiliconLoadoutSystem : EntitySystem
{
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private SiliconLawSystem _laws = default!;

    private ProtoId<LoadoutGroupPrototype> CyborgBrainLoadoutPrototype => "CyborgBrain";
    private ProtoId<LoadoutGroupPrototype> StationAiLawsetLoadoutPrototype => "StationAiLawset"; // SV

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BorgChassisComponent, CdPlayerSpawnBeforeMindEvent>(OnBorgPlayerSpawnComplete);
        SubscribeLocalEvent<StationAiHeldComponent, CdPlayerSpawnBeforeMindEvent>(OnAIPlayerSpawnComplete); // SV
    }

    private void OnBorgPlayerSpawnComplete(Entity<BorgChassisComponent> ent, ref CdPlayerSpawnBeforeMindEvent args)
    {
        // SV addition
        if (!TryGetSiliconLoadout(args.JobId,
                args.Character,
                args.Player,
                args.Character.Species,
                CyborgBrainLoadoutPrototype,
                out var roleLoadout))
            return;

        if (!_container.TryGetContainer(ent, ent.Comp.BrainContainerId, out var container) ||
            !_proto.TryIndex(roleLoadout.Prototype, out var loadoutProto))
            return;

        // we only run if we're absolutely SURE that we have a proto to replace it with
        foreach (var brain in _container.EmptyContainer(container))
        {
            QueueDel(brain);
        }

        TrySpawnInContainer(loadoutProto.Brain, ent, ent.Comp.BrainContainerId, out var mmi);
    }

    // SV start
    private void OnAIPlayerSpawnComplete(Entity<StationAiHeldComponent> ent, ref CdPlayerSpawnBeforeMindEvent args)
    {
        if (!TryGetSiliconLoadout(args.JobId,
                args.Character,
                args.Player,
                args.Character.Species,
                StationAiLawsetLoadoutPrototype,
                out var roleLoadout))
            return;

        if(!_proto.TryIndex(roleLoadout.Prototype, out var loadoutProto) ||
           loadoutProto.Lawset is not {} laws)
            return;

        var lawset = _laws.GetLawset(laws);
        _laws.SetLaws(lawset.Laws, ent);
    }

    private bool TryGetSiliconLoadout(ProtoId<JobPrototype> jobId,
        HumanoidCharacterProfile profile,
        ICommonSession player,
        ProtoId<SpeciesPrototype> species,
        ProtoId<LoadoutGroupPrototype> loadoutGroupProtoKey,
        [NotNullWhen(true)] out Loadout? loadout)
    {
        var jobLoadoutId = LoadoutSystem.GetJobPrototype(jobId);
        var selectedLoadouts = profile
            .GetLoadoutOrDefault(jobLoadoutId, player, species, EntityManager, _proto)
            .SelectedLoadouts[loadoutGroupProtoKey];

        Debug.Assert(selectedLoadouts.Count == 1);
        if (!selectedLoadouts.TryFirstOrDefault(out loadout))
            return false;

        return true;
    }
    // SV end
}

/// <summary>
/// Event that raises right before the mind is added to a player's entity while spawning on station.
/// </summary>
public sealed record CdPlayerSpawnBeforeMindEvent(ICommonSession Player, HumanoidCharacterProfile Character, ProtoId<JobPrototype> JobId);
