using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.Licences;

public sealed partial class LicenceSystem : EntitySystem
{
    // [Dependency] private PrototypeManager _protoman = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public CargoProductPrototype TryMakeProductVisible(CargoProductPrototype cargoProduct)
    {
        if (!cargoProduct.Visible)
        {
            cargoProduct.Visible = true;
            return cargoProduct;
        }
        Log.Info($"{cargoProduct.ID} was already visible ignoring");
        return cargoProduct;
    }
}
