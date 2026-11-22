using Content.Shared.Silicons.Laws;
using Robust.Shared.Prototypes;

// ReSharper disable once CheckNamespace
namespace Content.Shared.Preferences.Loadouts;

public sealed partial class LoadoutPrototype
{
    [DataField]
    public EntProtoId? Brain { get; set; } = new();

    [DataField]
    public ProtoId<SiliconLawsetPrototype>? Lawset { get; set; } = new(); // SV
}
