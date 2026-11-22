using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Tag;
using Content.Shared.Humanoid;
using Content.Shared._NF.Clothing.Components; // Frontier

namespace Content.Shared._DV.Harpy;

public sealed partial class HarpyVisualsSystem : EntitySystem
{
//    [Dependency] private readonly TagSystem _tagSystem = default!; //SV: Unused causing errors
    [Dependency] private SharedHideableHumanoidLayersSystem _hideableHumanoidLayers = default!;

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
