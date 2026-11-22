using Robust.Shared.Utility;

namespace Content.Shared.Maps;

// SV - Extra map prototype data for Sector Vestige.
public sealed partial class GameMapPrototype
{
    /// <summary>
    /// SV - Optional path to a small preview image of the map, i.e. `/Textures/_SV/MapPreviews/amber.png`.
    /// Shown on the vote buttons of the mapvotesv command.
    /// </summary>
    [DataField]
    public ResPath? Preview;
}
