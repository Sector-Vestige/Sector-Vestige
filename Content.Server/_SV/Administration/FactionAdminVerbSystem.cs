using System.Linq;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Database;
using Content.Shared.Mobs.Components;
using Content.Shared.NPC.Components;
using Content.Shared.NPC.Prototypes;
using Content.Shared.NPC.Systems;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._SV.Administration;

/// <summary>
///     Adds an admin "Factions" submenu to the right-click verb menu, letting GMs toggle an
///     entity's <see cref="NpcFactionMemberComponent"/> membership of any <see cref="NpcFactionPrototype"/>
///     and clear all factions at once. Lets admins swap player characters and NPCs between factions
///     without hand-editing the component in View Variables.
/// </summary>
public sealed partial class FactionAdminVerbSystem : EntitySystem
{
    [Dependency] private IAdminManager _adminManager = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private NpcFactionSystem _faction = default!;

    private static readonly VerbCategory FactionCategory =
        new("verb-categories-faction", "/Textures/Interface/VerbIcons/group.svg.192dpi.png");

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GetVerbsEvent<Verb>>(AddFactionVerbs);
    }

    private void AddFactionVerbs(GetVerbsEvent<Verb> args)
    {
        if (!TryComp<ActorComponent>(args.User, out var actor))
            return;

        if (!_adminManager.HasAdminFlag(actor.PlayerSession, AdminFlags.Admin))
            return;

        // Only offer this on mobs/players or anything that already tracks factions, to avoid
        // cluttering the verb menu of every entity in the game.
        if (!HasComp<MobStateComponent>(args.Target) && !HasComp<NpcFactionMemberComponent>(args.Target))
            return;

        var target = args.Target;

        // Toggle membership of every faction. Members show a checkmark and clicking removes them;
        // non-members show no mark and clicking adds them. This covers add / remove / multiple.
        foreach (var faction in _proto.EnumeratePrototypes<NpcFactionPrototype>().OrderBy(f => f.ID))
        {
            var id = faction.ID;
            var isMember = _faction.IsMember(target, id);

            Verb verb = new()
            {
                Text = isMember ? $"✓ {id}" : id,
                Category = FactionCategory,
                Priority = isMember ? 1 : 0,
                Act = () =>
                {
                    if (_faction.IsMember(target, id))
                        _faction.RemoveFaction(target, id);
                    else
                        _faction.AddFaction(target, id);
                },
                Impact = LogImpact.Medium,
                Message = Loc.GetString(
                    isMember ? "admin-verb-faction-remove-message" : "admin-verb-faction-add-message",
                    ("faction", id)),
            };
            args.Verbs.Add(verb);
        }

        // Clear everything in one click. Sorted to the top of the category.
        Verb clear = new()
        {
            Text = Loc.GetString("admin-verb-faction-clear-text"),
            Category = FactionCategory,
            Priority = 10,
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/delete.svg.192dpi.png")),
            Act = () => _faction.ClearFactions(target),
            Impact = LogImpact.Medium,
            Message = Loc.GetString("admin-verb-faction-clear-message"),
        };
        args.Verbs.Add(clear);
    }
}
