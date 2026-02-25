// SPDX-FileCopyrightText: 2026 Delta-V contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 V <97265903+formlessnameless@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

/*
* Delta-V - This file is licensed under AGPLv3
* Copyright (c) 2024 Delta-V Contributors
* See AGPLv3.txt for details.
*/

using Content.Server.StationEvents.Events;

namespace Content.Server.StationEvents.Components;

/// <summary>
/// Spawns random debris in space around a loaded grid.
/// Requires <see cref="LoadFarGridRuleComponent"/>.
/// </summary>
[RegisterComponent, Access(typeof(DebrisSpawnerRule))]
public sealed partial class DebrisSpawnerRuleComponent : Component
{
    /// <summary>
    /// How many debris grids to spawn.
    /// </summary>
    [DataField(required: true)]
    public int Count;

    /// <summary>
    /// Modifier for debris distance.
    /// Should be between 3 and 10 generally.
    /// </summary>
    [DataField(required: true)]
    public float DistanceModifier;
}
