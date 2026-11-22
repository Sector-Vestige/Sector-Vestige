using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.Weapons.Hitscan;

/// <summary>
/// Hitscan entities with this component fire chain lightning when they strike a target:
/// optionally a visible bolt from the shooter to the target, then arcs onward to nearby targets.
/// Listens for the same hit event as the other hitscan effect components.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class HitscanLightningArcComponent : Component
{
    /// <summary>
    /// The lightning entity prototype to spawn for each bolt.
    /// </summary>
    [DataField]
    public EntProtoId LightningPrototype = "Lightning";

    /// <summary>
    /// If true, fire a lightning beam from the shooter to the struck target (the visible "shot").
    /// </summary>
    [DataField]
    public bool ShootPrimaryBeam = true;

    /// <summary>
    /// Radius around the struck target searched for chain-lightning victims.
    /// </summary>
    [DataField]
    public float Range = 4f;

    /// <summary>
    /// Number of chain bolts fired from the struck target.
    /// </summary>
    [DataField]
    public int BoltCount = 2;

    /// <summary>
    /// How many times the chain can bounce onward from each struck target.
    /// </summary>
    [DataField]
    public int ArcDepth = 1;
}
