using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._SV.Licences;

[Prototype]
public sealed partial class LicencePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string Name = string.Empty;

    [DataField]
    public SpriteSpecifier Icon { get; private set; } = SpriteSpecifier.Invalid;

    [DataField]
    public string Description = string.Empty;

    [DataField]
    public List<ProtoId<CargoProductPrototype>> Products = new();

    [DataField(required: true)]
    public ProtoId<CargoAccountPrototype> CargoAccount;
}
