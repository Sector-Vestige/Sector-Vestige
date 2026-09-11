using Content.Shared._SV.Utility;
using Robust.Shared.GameStates;

namespace Content.Shared._SV.Fire;

/// <summary>
/// This is used for how reagents should react to being burned. This component should house data for the fire to work, and update only when the composition of the puddle changes.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class FlammableFluidComponent : Component
{
    /// <summary>
    /// Should the reagent be flammable
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool IsFlammable;

    /// <summary>
    /// Is the reagent an oxidizer
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool IsOxidizer;

    /// <summary>
    /// How much heat, in Joules, does the reagent produce when 1u of fluid is burnt
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float GeneratedHeat;

    /// <summary>
    /// What is the maximum temperature the reagent fire should reach
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float MaxHeat;

    /// <summary>
    /// The list of gasses that the fluid should produce when burnt
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public List<GasSpawnEntry>? ExhaustedGases;

    /// <summary>
    /// Should the reagent try to light the puddle on fire by itself?
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public bool DoesAutoIgnite;
}
