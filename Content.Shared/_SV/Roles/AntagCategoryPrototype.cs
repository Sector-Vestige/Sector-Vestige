// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared._SV.Roles;

/// <summary>
/// Antag category with general settings. Allows you to group antagonist preferences by faction or type
/// </summary>
[Prototype]
public sealed partial class AntagCategoryPrototype : IPrototype
{
    public const string Default = "Default";

    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    ///     Name of the antag category displayed in the UI
    /// </summary>
    [DataField]
    public LocId Name { get; private set; } = string.Empty;
}
