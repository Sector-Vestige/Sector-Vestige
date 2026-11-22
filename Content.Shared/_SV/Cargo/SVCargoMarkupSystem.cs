using Content.Shared._SV.CCVar;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Configuration;

namespace Content.Shared._SV.Cargo;

public sealed partial class SVCargoMarkupSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = default!;

    public int GetPrice(CargoProductPrototype product)
    {
        var percentage = _cfg.GetCVar(SVCCVars.CargoMarkupPercent);
        return product.Cost + product.Cost * percentage / 100;
    }
}
