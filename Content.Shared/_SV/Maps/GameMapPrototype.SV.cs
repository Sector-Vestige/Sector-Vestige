// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
