using Content.Server._SV.StationEvents.Events;
using Content.Shared.Atmos;
using Content.Shared._SV.Utility;

namespace Content.Server._SV.StationEvents.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, Access(typeof(SMOffGasRule))]
public sealed partial class SMOffGasComponent : Component
{
    /// <summary>
    /// A list of gases, their weights, spawn amounts, and spawn rates for the internal process to select
    /// </summary>
    [DataField("allowedGases")]
    public List<GasSpawnEntry> AllowedGases = new();

    /// <summary>
    /// The type of gas for the SM to off gas
    /// </summary>
    [DataField]
    public Gas SelectedGas;

    /// <summary>
    /// How fast the gas gets emitted
    /// </summary>
    [DataField]
    public int GasAmount;

    /// <summary>
    /// How long until the next leak event occurs
    /// </summary>
    [DataField]
    public float TimeTillNextLeak = 1f;

    /// <summary>
    /// How fast the gas will be produced (Likely will be redundant)
    /// </summary>
    [DataField]
    public int GasRate;

    /// <summary>
    /// What temperature the gas should be coming out of the crystal
    /// </summary>
    [DataField]
    public float GasTemp = 293.15f;

    /// <summary>
    /// Where is the gas going to be spawned?
    /// </summary>
    [DataField]
    public Vector2i TargetTile;

    /// <summary>
    /// The station that the leak is happening on
    /// </summary>
    [DataField]
    public EntityUid StationUid;

    /// <summary>
    /// The supermatter that was selected
    /// </summary>
    [DataField]
    public EntityUid Supermatter;

    /// <summary>
    /// The target grid for the event
    /// </summary>
    [DataField]
    public EntityUid? TargetGrid;
}
