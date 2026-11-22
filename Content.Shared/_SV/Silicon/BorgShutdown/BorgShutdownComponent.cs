using Robust.Shared.GameStates;

namespace Content.Shared._SV.Silicon.BorgShutdown;

/// <summary>
/// SV: Component that allows borgs to shutdown (drain battery to 0) and restore battery charge.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BorgShutdownComponent : Component
{
    /// <summary>
    /// Whether the borg is currently shut down.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsShutdown;

    /// <summary>
    /// The stored battery charge before shutdown, used to restore on wake up.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float StoredCharge;
}
