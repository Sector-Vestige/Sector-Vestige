using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared._SV.Objectives;

/// <summary>
/// A named icon that admins can pick when writing a custom objective, so they don't have to
/// remember rsi paths in the middle of a round. Add entries to custom_icons.yml to offer more.
/// </summary>
[Prototype]
public sealed partial class CustomObjectiveIconPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// The sprite shown next to the objective in the character menu.
    /// </summary>
    [DataField(required: true)]
    public SpriteSpecifier Icon = default!;
}
