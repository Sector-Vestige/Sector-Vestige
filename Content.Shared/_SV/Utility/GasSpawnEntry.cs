using Content.Shared.Atmos;
using Content.Shared.Destructible.Thresholds;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.Utility;

/// <summary>
/// This is a prototype for...
/// </summary>
[Prototype]
public sealed partial class GasSpawnEntryPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = string.Empty;

    [DataField]
    public List<GasSpawnEntry> Entry = new();
}

/// <summary>
/// Dictates a list of gases that the supermatter off gas event can spawn, along with their probabilities and amount of gas emissions.
/// <example>
/// <code>
///    - gas: Nitrogen
///      spawnWeight: 8
///      gasAmount:
///        min: 400
///        max: 1100
///      molPerSecond:
///       min: 10
///       max: 50
/// </code>
/// </example>
/// </summary>

[Serializable]
[DataDefinition]
public partial struct GasSpawnEntry
{
    [DataField("gas")]
    public Gas Gas = Gas.Nitrogen;

    [DataField("spawnWeight")]
    public int Weight = 1;

    [DataField("gasAmount")]
    public MinMax Amount = new(200, 300);

    [DataField]
    public MinMax MolPerSecond = new(10, 30);

    public GasSpawnEntry() { }
}

