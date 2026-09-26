using Content.Shared.RCD.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.RPD;

[RegisterComponent, NetworkedComponent]
[Access(typeof(RCDSystem))]
public sealed partial class RPDDeconstructableComponent : Component
{
    /// <summary>
    /// Number of charges consumed when the deconstruction is completed
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public int Cost = 1;

    /// <summary>
    /// The length of the deconstruction
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Delay;

    /// <summary>
    /// The visual effect that plays during deconstruction
    /// </summary>
    [DataField("fx"), ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId? Effect;

    /// <summary>
    /// Toggles whether this entity is deconstructable or not
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool Deconstructable = true;
}
