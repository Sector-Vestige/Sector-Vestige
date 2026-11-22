using Content.Shared._SV.Utility;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._SV.Fire;

/// <summary>
/// This is used for the fire itself that is burning the puddle...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentPause, AutoGenerateComponentState]
public sealed partial class ActualFireComponent : Component
{
    /// <summary>
    /// The UID of the solution that is being burnt
    /// </summary>
    [DataField]
    public EntityUid? TargetEntity;

    /// <summary>
    /// How much heat the fire will expel each fire tick
    /// </summary>
    [DataField]
    public float GenratedHeat;

    /// <summary>
    /// The total temperature the fire will burn upto
    /// </summary>
    [DataField]
    public float MaxFireTemp;

    /// <summary>
    /// A ratio of how much oxidizer is in the fluid being burnt.
    /// A number below 1 would require oxygen from the atmosphere to be burnt at full temperature, and a value higher than 1 would allow the fire to burn at a higher temperature, but faster.
    /// If there was 15 units of Oxygen, and 15 units of Oil, the Oxidation should be 15, and burn at normal temperature
    /// This would also take into account the atmosphere of the tile
    /// </summary>
    [DataField]
    public float Oxidation;

    /// <summary>
    /// The list of gases that should be spawned for every fire tick
    /// </summary>
    [DataField]
    public GasSpawnEntry[]? GasSpawnEntries;

    /// <summary>
    /// How long in-between fire ticks should there be
    /// Default is 1 second
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float TimeBetweenFireTick = 1f;

    /// <summary>
    /// When the next fire tick should happen
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField, AutoNetworkedField, ViewVariables(VVAccess.ReadOnly)]
    public TimeSpan TimeTillNextTick = TimeSpan.Zero;
}
