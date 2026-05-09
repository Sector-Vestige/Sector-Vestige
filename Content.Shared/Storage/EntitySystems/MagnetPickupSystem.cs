// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Stray-Pyramid <Pharaohofnile@gmail.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2025 kosticia <kosticia46@gmail.com>
// SPDX-FileCopyrightText: 2026 s3ptemb3rr (GitHub)
// SPDX-FileCopyrightText: 2026 september <rdamico2204@gmail.com>
//
// SPDX-License-Identifier: MIT

// using Content.Server.Storage.Components;
using Content.Shared.Clothing.Components;    // Frontier
using Content.Shared.Examine;   // Frontier
using Content.Shared.Hands.Components;  // Frontier
using Content.Shared.Inventory;
using Content.Shared.Verbs;     // Frontier
using Content.Shared.Storage.Components;    // Frontier
using Content.Shared.Whitelist;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Utility;    // Frontier

namespace Content.Shared.Storage.EntitySystems;

/// <summary>
/// <see cref="MagnetPickupComponent"/>
/// </summary>
public sealed class MagnetPickupSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedStorageSystem _storage = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;

    private static readonly TimeSpan ScanDelay = TimeSpan.FromSeconds(1);

    private EntityQuery<PhysicsComponent> _physicsQuery;

    public override void Initialize()
    {
        base.Initialize();
        _physicsQuery = GetEntityQuery<PhysicsComponent>();
        SubscribeLocalEvent<MagnetPickupComponent, MapInitEvent>(OnMagnetMapInit);
        SubscribeLocalEvent<MagnetPickupComponent, ExaminedEvent>(OnExamined);  // Frontier
        SubscribeLocalEvent<MagnetPickupComponent, GetVerbsEvent<AlternativeVerb>>(AddToggleMagnetVerb);    // Frontier
    }

    private void OnMagnetMapInit(EntityUid uid, MagnetPickupComponent component, MapInitEvent args)
    {
        component.NextScan = _timing.CurTime;
    }

    // Frontier, used to add the magnet toggle to the context menu
    private void AddToggleMagnetVerb(EntityUid uid, MagnetPickupComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract)
            return;

        if (!HasComp<HandsComponent>(args.User))
            return;

        AlternativeVerb verb = new()
        {
            Act = () =>
            {
                ToggleMagnet(uid, component);
            },
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/Spare/poweronoff.svg.192dpi.png")),
            Text = Loc.GetString("magnet-pickup-component-toggle-verb"),
            Priority = 3
        };

        args.Verbs.Add(verb);
    }

    // Frontier, used to show the magnet state on examination
    private void OnExamined(EntityUid uid, MagnetPickupComponent component, ExaminedEvent args)
    {
        args.PushMarkup(Loc.GetString("magnet-pickup-component-on-examine-main",
                        ("stateText", Loc.GetString(component.MagnetEnabled
                        ? "magnet-pickup-component-magnet-on"
                        : "magnet-pickup-component-magnet-off"))));
    }

    // Frontier, used to toggle the magnet on the ore bag/box
    public bool ToggleMagnet(EntityUid uid, MagnetPickupComponent comp)
    {
        var query = EntityQueryEnumerator<MagnetPickupComponent>();
        comp.MagnetEnabled = !comp.MagnetEnabled;

        return comp.MagnetEnabled;
    }


    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<MagnetPickupComponent, StorageComponent, TransformComponent>();
        var currentTime = _timing.CurTime;

        while (query.MoveNext(out var uid, out var comp, out var storage, out var xform))
        {
            if (comp.NextScan > currentTime)
                continue;

            comp.NextScan += ScanDelay;

            // No space
            if (!_storage.HasSpace((uid, storage)))
                continue;

            // Frontier - magnet disabled
            if (!comp.MagnetEnabled)
                continue;

            // Frontier - is ore bag on belt?
            if (HasComp<ClothingComponent>(uid))
            {
                if (!_inventory.TryGetContainingSlot(uid, out var slotDef))
                    continue;

                if ((slotDef.SlotFlags & comp.SlotFlags) == 0x0)
                    continue;
            }

            var parentUid = xform.ParentUid;
            var playedSound = false;
            var finalCoords = xform.Coordinates;
            var moverCoords = _transform.GetMoverCoordinates(uid, xform);

            foreach (var near in _lookup.GetEntitiesInRange(uid, comp.Range, LookupFlags.Dynamic | LookupFlags.Sundries))
            {
                if (_whitelist.IsWhitelistFail(storage.Whitelist, near))
                    continue;

                if (!_physicsQuery.TryGetComponent(near, out var physics) || physics.BodyStatus != BodyStatus.OnGround)
                    continue;

                if (near == parentUid)
                    continue;

                // TODO: Probably move this to storage somewhere when it gets cleaned up
                // TODO: This sucks but you need to fix a lot of stuff to make it better
                // the problem is that stack pickups delete the original entity, which is fine, but due to
                // game state handling we can't show a lerp animation for it.
                var nearXform = Transform(near);
                var nearMap = nearXform.MapPosition;
                var nearCoords = EntityCoordinates.FromMap(moverCoords.EntityId, nearMap, _transform, EntityManager);

                if (!_storage.Insert(uid, near, out var stacked, storageComp: storage, playSound: !playedSound))
                    continue;

                // Play pickup animation for either the stack entity or the original entity.
                if (stacked != null)
                    _storage.PlayPickupAnimation(stacked.Value, nearCoords, finalCoords, nearXform.LocalRotation);
                else
                    _storage.PlayPickupAnimation(near, nearCoords, finalCoords, nearXform.LocalRotation);

                playedSound = true;
            }
        }
    }
}
