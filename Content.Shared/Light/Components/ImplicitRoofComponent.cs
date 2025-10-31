using Robust.Shared.GameStates;

namespace Content.Shared.Light.Components;

/// <summary>
<<<<<<< HEAD
/// Assumes the entire attached grid is rooved.
=======
/// Assumes the entire attached grid is rooved. This component will get removed if the grid has RoofComponent.
>>>>>>> upstream/master
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ImplicitRoofComponent : Component
{
    [DataField, AutoNetworkedField]
    public Color Color = Color.Black;
}
