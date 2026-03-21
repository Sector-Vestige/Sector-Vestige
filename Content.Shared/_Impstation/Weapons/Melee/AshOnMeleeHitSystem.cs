// SPDX-FileCopyrightText: 2026 Impstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 beck <163376292+widgetbeck@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._EE.Supermatter.Components;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio.Systems;

namespace Content.Shared._Impstation.Weapons.Melee;

public sealed class AshOnMeleeHitSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AshOnMeleeHitComponent, MeleeHitEvent>(OnMeleeHit);
        SubscribeLocalEvent<AshOnMeleeHitComponent, ThrowDoHitEvent>(OnThrowHit);
    }

    private void OnMeleeHit(Entity<AshOnMeleeHitComponent> ent, ref MeleeHitEvent args)
    {
        if (args.Handled || args.HitEntities.Count < 1)
            return;

        var ashed = 0;

        foreach (var target in args.HitEntities)
        {
            if (HasComp<SupermatterImmuneComponent>(target))
                return;

            Ash(ent, target);
            ashed++;
        }

        if (ashed == 0)
            return;

        _audio.PlayPvs(ent.Comp.Sound, Transform(ent).Coordinates);

        if (ent.Comp.SingleUse)
            QueueDel(ent);
    }

    private void OnThrowHit(Entity<AshOnMeleeHitComponent> ent, ref ThrowDoHitEvent args)
    {
        if (HasComp<SupermatterImmuneComponent>(args.Target))
            return;

        Ash(ent, args.Target);
        _audio.PlayPvs(ent.Comp.Sound, Transform(ent).Coordinates);

        if (ent.Comp.SingleUse)
            QueueDel(ent);
    }

    private void Ash(Entity<AshOnMeleeHitComponent> ent, EntityUid target)
    {
        var coords = Transform(target).Coordinates;

        _popup.PopupCoordinates(Loc.GetString(ent.Comp.Popup, ("entity", ent.Owner), ("target", target)), coords, PopupType.LargeCaution);

        Spawn(ent.Comp.AshPrototype, coords);
        QueueDel(target);
    }
}
