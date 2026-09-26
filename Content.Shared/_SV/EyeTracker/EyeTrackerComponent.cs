using Robust.Shared.GameStates;

namespace Content.Shared._SV.EyeTracker;

[RegisterComponent, NetworkedComponent]
public sealed partial class EyeTrackerComponent : Component
{
    /// <summary>
    /// The current rotation of the user camera
    /// This is a stupid component but there isn't a better way of doing it
    /// </summary>
    [DataField]
    public Angle Rotation = 0;
}
