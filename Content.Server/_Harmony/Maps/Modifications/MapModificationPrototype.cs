// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2025 youtissoum <51883137+youtissoum@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

﻿using System.Numerics;
using Content.Shared.Maps;
using Content.Shared._Harmony.EntitySelector;
using Robust.Shared.Prototypes;

namespace Content.Server._Harmony.Maps.Modifications;

/// <remarks>
/// Map modifications are processed in the following order:
/// <list type="number">
/// <item>removals</item>
/// <item>replacements</item>
/// <item>additions</item>
/// </list>
///
/// </remarks>
[Prototype]
public sealed partial class MapModificationPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// A <see cref="GameMapPrototype"/> ID to automatically apply this addition to.
    /// </summary>
    [DataField]
    public List<ProtoId<GameMapPrototype>> ApplyOn = new();

    /// <summary>
    /// A list of entities that should be added to the map.
    /// </summary>
    [DataField]
    public List<MapModificationEntity> Additions = new();

    /// <summary>
    /// A list of <see cref="EntitySelector"/>s that defined the entities that should be deleted from the map.
    /// </summary>
    [DataField]
    public List<EntitySelector> Removals = new();

    /// <summary>
    /// A list of <see cref="MapModificationReplacement"/>s that can replace entities;
    /// </summary>
    [DataField]
    public List<MapModificationReplacement> Replacements = new();
}

[DataDefinition]
public sealed partial class MapModificationReplacement
{
    /// <summary>
    /// An <see cref="EntitySelector"/> that defines which entities should be replaced.
    /// </summary>
    [DataField(required: true)]
    public List<EntitySelector> From = new();

    [DataField(required: true)]
    public EntProtoId NewPrototype;

    [DataField]
    public string? NewName;

    [DataField]
    public string? NewDescription;

    [DataField]
    public Angle? NewRotation;

    [DataField]
    public ComponentRegistry? NewComponents;
}

[DataDefinition]
public sealed partial class MapModificationEntity
{
    [DataField(required: true)]
    public EntProtoId Prototype;

    [DataField]
    public string? Name;

    [DataField]
    public string? Description;

    [DataField(required: true)]
    public Vector2 Position;

    [DataField]
    public Angle? Rotation;

    [DataField]
    public ComponentRegistry? Components;
}
