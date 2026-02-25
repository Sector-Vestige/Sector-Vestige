// SPDX-FileCopyrightText: 2026 Delta-V contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 V <97265903+formlessnameless@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.StationEvents.Events;
using Robust.Shared.Utility;

namespace Content.Server.StationEvents.Components;

/// <summary>
/// Loads a grid far away from a random station.
/// Requires <see cref="RuleGridsComponent"/>.
/// </summary>
[RegisterComponent, Access(typeof(LoadFarGridRule))]
public sealed partial class LoadFarGridRuleComponent : Component
{
    /// <summary>
    /// Path to the grid to spawn.
    /// </summary>
    [DataField(required: true)]
    public ResPath Path = new();

    /// <summary>
    /// Roughly how many AABBs away
    /// </summary>
    [DataField(required: true)]
    public float DistanceModifier;

    /// <summary>
    /// "Stations of Unusual Size Constant", derived from the AABB.Width of Shoukou.
    /// This Constant is used to check the size of a station relative to the reference point
    /// </summary>
    [DataField]
    public float Sousk = 123.44f;
}
