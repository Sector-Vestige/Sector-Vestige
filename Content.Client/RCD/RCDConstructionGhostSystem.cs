// SPDX-FileCopyrightText: 2025 Wizards Den contributors
// SPDX-FileCopyrightText: 2025 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 August Eymann <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2024 chromiumboy <50505512+chromiumboy@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <131422822+OnyxTheBrave@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <vinjeerik@gmail.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2025 gus <august.eymann@gmail.com>
// SPDX-FileCopyrightText: 2025 jajsha <corbinbinouche7@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Client._SV.RPD; //Sector Vestige: Used for displaying RPD pipe layers
using Content.Shared.Input; //Sector Vestige: Used for displaying RPD pipe layers
using Content.Client.Hands.Systems;
using Content.Shared.Interaction;
using Content.Shared.RCD;
using Content.Shared.RCD.Components;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Input; //Sector Vestige: Used for flipping the prototypes for gas mixers or gas filters
using Robust.Shared.Input.Binding; //Sector Vestige: Used for flipping the prototypes for gas mixers or gas filters

namespace Content.Client.RCD;

/// <summary>
/// System for handling structure ghost placement in places where RCD can create objects.
/// </summary>
public sealed partial class RCDConstructionGhostSystem : EntitySystem
{
    private const string PlacementMode = nameof(AlignRCDConstruction);
    private const string RPDPlacementMode = nameof(AlignRPDPipeLayers); //Sector Vestige: RPD logic

    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IPlacementManager _placementManager = default!;
    [Dependency] private HandsSystem _hands = default!;

    private Direction _placementDirection = default;

    //Sector Vestige - Begin: Logic to get the RPD to flip the prototype.
    public override void Initialize()
    {
        base.Initialize();

        // bind key
        CommandBinds.Builder
            .Bind(ContentKeyFunctions.EditorFlipObject,
                new PointerInputCmdHandler(HandleFlip, outsidePrediction: true))
            .Register<RCDConstructionGhostSystem>();
    }

    public override void Shutdown()
    {
        CommandBinds.Unregister<RCDConstructionGhostSystem>();
        base.Shutdown();
    }

    private bool HandleFlip(in PointerInputCmdHandler.PointerInputCmdArgs args)
    {
        if (args.State == BoundKeyState.Down)
        {
            if (!_placementManager.IsActive || _placementManager.Eraser)
                return false;

            var placerEntity = _placementManager.CurrentPermission?.MobUid;

            if (!TryComp<RCDComponent>(placerEntity, out var rcd) ||
                string.IsNullOrEmpty(ProtoMan.Index(rcd.ProtoId).FlippedPrototype))
                return false;

            var prototype = ProtoMan.Index(rcd.ProtoId);

            var useProto = rcd.UseFlippedPrototype && !string.IsNullOrEmpty(prototype.FlippedPrototype)
                ? prototype.FlippedPrototype
                : prototype.Prototype;

            RaiseNetworkEvent(new RCDConstructionGhostFlipEvent(GetNetEntity(placerEntity.Value)));
            CreatePlacer(placerEntity.Value, rcd, useProto);
        }
        return true;
    }
    //Sector Vestige - Begin: Logic to get the RPD to flip the prototype.

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Get current placer data
        var placerEntity = _placementManager.CurrentPermission?.MobUid;
        var placerProto = _placementManager.CurrentPermission?.EntityType;
        var placerIsRCD = HasComp<RCDComponent>(placerEntity);

        // Exit if erasing or the current placer is not an RCD (build mode is active)
        if (_placementManager.Eraser || (placerEntity != null && !placerIsRCD))
            return;

        // Determine if player is carrying an RCD in their active hand
        if (_playerManager.LocalSession?.AttachedEntity is not { } player)
            return;

        var heldEntity = _hands.GetActiveItem(player);

        // Don't open the placement overlay for client-side RCDs.
        // This may happen when predictively spawning one in your hands.
        if (heldEntity != null && IsClientSide(heldEntity.Value))
            return;

        if (!TryComp<RCDComponent>(heldEntity, out var rcd))
        {
            // If the player was holding an RCD, but is no longer, cancel placement
            if (placerIsRCD)
                _placementManager.Clear();

            return;
        }
        var prototype = ProtoMan.Index(rcd.ProtoId);

        // Update the direction the RCD prototype based on the placer direction
        if (_placementDirection != _placementManager.Direction)
        {
            _placementDirection = _placementManager.Direction;
            RaiseNetworkEvent(new RCDConstructionGhostRotationEvent(GetNetEntity(heldEntity.Value), _placementDirection));
        }

        var useProto = rcd.UseFlippedPrototype && !string.IsNullOrEmpty(prototype.FlippedPrototype) //Sector Vestige: Use the flipped prototype if it is called for, and if there is a flipped prototype provided
            ? prototype.FlippedPrototype //Sector Vestige: Use the flipped prototype if it is called for, and if there is a flipped prototype provided
            : prototype.Prototype; //Sector Vestige: Use the flipped prototype if it is called for, and if there is a flipped prototype provided

        // If the placer has not changed, exit
        if (heldEntity == placerEntity && useProto == placerProto) //Sector Vestige: Use the flipped prototype if it is called for, and if there is a flipped prototype provided
            return;

        //Sector Vestige - Begin: RPD Logic
        if (rcd.UseFlippedPrototype &&
            prototype.FlippedPrototype != null)
            CreatePlacer(heldEntity.Value, rcd, prototype.FlippedPrototype);
        else
            CreatePlacer(heldEntity.Value, rcd, prototype.Prototype);
    }

    private void CreatePlacer(EntityUid uid, RCDComponent rcd, string? prototype)
    {
    //If the entity that is being spawned is a pipe, use the AlignAtmosPipeLayers placement system
        PlacementInformation? newObjInfo = null;
        switch (ProtoMan.Index(rcd.ProtoId).Rotation)
        {
            // Create a new placer
            case RcdRotation.Camera:
            case RcdRotation.Fixed:
            case RcdRotation.User:
                newObjInfo = new PlacementInformation
                {
                    MobUid = uid,
                    PlacementOption = PlacementMode,
                    EntityType = prototype,
                    Range = (int)Math.Ceiling(SharedInteractionSystem.InteractionRange),
                    IsTile = (ProtoMan.Index(rcd.ProtoId).Mode == RcdMode.ConstructTile),
                    UseEditorContext = false,
                };
                    break;

            case RcdRotation.Pipe:
                newObjInfo = new PlacementInformation
                {
                    MobUid = uid,
                    PlacementOption = RPDPlacementMode,
                    EntityType = prototype,
                    Range = (int)Math.Ceiling(SharedInteractionSystem.InteractionRange),
                    UseEditorContext = false,
                };
                break;

        }

        if  (newObjInfo == null)
            return;
        //Sector Vestige - End: RPD Logic

        _placementManager.Clear();
        _placementManager.BeginPlacing(newObjInfo);
    }
}
