// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 PJB3005 <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2025 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Vasilis The Pikachu <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2025 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 dffdff2423 <dffdff2423@gmail.com>
// SPDX-FileCopyrightText: 2025 lunarcomets <140772713+lunarcomets@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 lunarcomets <lunarcomets2@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared._CD.Silicons.Borgs;
using Content.Shared.Actions;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Silicons.Borgs.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared.Silicons.Borgs;

/// <summary>
/// Implements borg type switching.
/// </summary>
/// <seealso cref="BorgSwitchableTypeComponent"/>
public abstract partial class SharedBorgSwitchableTypeSystem : EntitySystem
{
    // TODO: Allow borgs to be reset to default configuration.

    [Dependency] private SharedActionsSystem _actionsSystem = default!;
    [Dependency] private SharedUserInterfaceSystem _userInterface = default!;
    [Dependency] private InteractionPopupSystem _interactionPopup = default!;

    public static readonly EntProtoId ActionId = "ActionSelectBorgType";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BorgSwitchableTypeComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<BorgSwitchableTypeComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<BorgSwitchableTypeComponent, BorgToggleSelectTypeEvent>(OnSelectBorgTypeAction);

        Subs.BuiEvents<BorgSwitchableTypeComponent>(BorgSwitchableTypeUiKey.SelectBorgType,
            sub =>
            {
                sub.Event<BorgSelectTypeMessage>(SelectTypeMessageHandler);
            });
    }

    //
    // UI-adjacent code
    //

    private void OnMapInit(Entity<BorgSwitchableTypeComponent> ent, ref MapInitEvent args)
    {
        _actionsSystem.AddAction(ent, ref ent.Comp.SelectTypeAction, ActionId);
        Dirty(ent);

        if (ent.Comp.SelectedBorgType != null)
        {
            SelectBorgModule(ent, ent.Comp.SelectedBorgType.Value);
        }
    }

    private void OnShutdown(Entity<BorgSwitchableTypeComponent> ent, ref ComponentShutdown args)
    {
        _actionsSystem.RemoveAction(ent.Owner, ent.Comp.SelectTypeAction);
    }

    private void OnSelectBorgTypeAction(Entity<BorgSwitchableTypeComponent> ent, ref BorgToggleSelectTypeEvent args)
    {
        if (args.Handled || !TryComp<ActorComponent>(ent, out var actor))
            return;

        args.Handled = true;

        _userInterface.TryToggleUi((ent.Owner, null), BorgSwitchableTypeUiKey.SelectBorgType, actor.PlayerSession);
    }

    private void SelectTypeMessageHandler(Entity<BorgSwitchableTypeComponent> ent, ref BorgSelectTypeMessage args)
    {
        if (ent.Comp.SelectedBorgType != null)
            return;

        if (!ProtoMan.HasIndex(args.Prototype))
            return;

        SelectBorgModule(ent, args.Prototype);
    }

    //
    // Implementation
    //

    protected virtual void SelectBorgModule(
        Entity<BorgSwitchableTypeComponent> ent,
        ProtoId<BorgTypePrototype> borgType)
    {
        ent.Comp.SelectedBorgType = borgType;

        _actionsSystem.RemoveAction(ent.Owner, ent.Comp.SelectTypeAction);
        ent.Comp.SelectTypeAction = null;
        Dirty(ent);

        _userInterface.CloseUi((ent.Owner, null), BorgSwitchableTypeUiKey.SelectBorgType);

        UpdateEntityAppearance(ent);

        // AL - event for subtype system, always runs at end of borg type code
        var ev = new AfterBorgTypeSelectEvent();
        RaiseLocalEvent(ent, ref ev);
    }

    protected void UpdateEntityAppearance(Entity<BorgSwitchableTypeComponent> entity)
    {
        if (!ProtoMan.Resolve(entity.Comp.SelectedBorgType, out var proto))
            return;

        UpdateEntityAppearance(entity, proto);
    }

    protected virtual void UpdateEntityAppearance(
        Entity<BorgSwitchableTypeComponent> entity,
        BorgTypePrototype prototype)
    {
        if (TryComp(entity, out InteractionPopupComponent? popup))
        {
            _interactionPopup.SetInteractSuccessString((entity.Owner, popup), prototype.PetSuccessString);
            _interactionPopup.SetInteractFailureString((entity.Owner, popup), prototype.PetFailureString);
        }

        if (TryComp(entity, out FootstepModifierComponent? footstepModifier))
        {
            footstepModifier.FootstepSoundCollection = prototype.FootstepCollection;
        }

        if (prototype.SpriteBodyMovementState is { } movementState)
        {
            var spriteMovement = EnsureComp<SpriteMovementComponent>(entity);
            spriteMovement.NoMovementLayers.Clear();
            spriteMovement.NoMovementLayers["movement"] = new PrototypeLayerData
            {
                State = prototype.SpriteBodyState,
            };
            spriteMovement.MovementLayers.Clear();
            spriteMovement.MovementLayers["movement"] = new PrototypeLayerData
            {
                State = movementState,
            };
        }
        else
        {
            RemComp<SpriteMovementComponent>(entity);
        }
    }
}
