using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._SV.Roles;

/// <summary>
/// Adds the listed NPC factions to the spawned mob, on top of whatever it already has.
/// Unlike <see cref="AddComponentSpecial"/> this is additive: it never clobbers the
/// existing <c>NpcFactionMember</c>, so a job can join a second faction without having
/// to relist its base ones.
/// </summary>
public sealed partial class AddFactionSpecial : JobSpecial
{
    [DataField(required: true)]
    public List<ProtoId<NpcFactionPrototype>> Factions = new();

    public override void AfterEquip(EntityUid mob)
    {
        var entMan = IoCManager.Resolve<IEntityManager>();
        var factions = entMan.System<NpcFactionSystem>();

        foreach (var faction in Factions)
        {
            factions.AddFaction(mob, faction);
        }
    }
}
