// SPDX-FileCopyrightText: 2026 Delta-V contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Tag;
using Content.Shared.Humanoid;
using Content.Shared._NF.Clothing.Components; // Frontier

namespace Content.Shared._DV.Harpy;

public sealed class HarpyVisualsSystem : EntitySystem
{
    [Dependency] private readonly TagSystem _tagSystem = default!;
    [Dependency] private readonly SharedHideableHumanoidLayersSystem _hideableHumanoidLayers = default!;

    //    [ValidatePrototypeId<TagPrototype>] // Frontier
    //    private const string HarpyWingsTag = "HidesHarpyWings"; // Frontier

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HarpySingerComponent, DidEquipEvent>(OnDidEquipEvent);
        SubscribeLocalEvent<HarpySingerComponent, DidUnequipEvent>(OnDidUnequipEvent);
    }

    private void OnDidEquipEvent(EntityUid uid, HarpySingerComponent component, DidEquipEvent args)
    {
        if (args.Slot == "outerClothing" && HasComp<HarpyHideWingsComponent>(args.Equipment)) // Frontier: Swap tag to comp
        {
            _hideableHumanoidLayers.SetLayerOcclusion(uid, HumanoidVisualLayers.RArm, true, SlotFlags.OUTERCLOTHING);
            _hideableHumanoidLayers.SetLayerOcclusion(uid, HumanoidVisualLayers.Tail, true, SlotFlags.OUTERCLOTHING);
        }
    }

    private void OnDidUnequipEvent(EntityUid uid, HarpySingerComponent component, DidUnequipEvent args)
    {
        if (args.Slot == "outerClothing" && HasComp<HarpyHideWingsComponent>(args.Equipment)) // Frontier: Swap tag to comp
        {
            _hideableHumanoidLayers.SetLayerOcclusion(uid, HumanoidVisualLayers.RArm, false, SlotFlags.OUTERCLOTHING);
            _hideableHumanoidLayers.SetLayerOcclusion(uid, HumanoidVisualLayers.Tail, false, SlotFlags.OUTERCLOTHING);
        }
    }
}
