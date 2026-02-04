// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Server._CD.Loadouts;

/// <summary>
/// Marker that should be attached to the PDA to rename the contained ID to the user's requested job. Used to implement custom tile loadouts.
/// </summary>
[RegisterComponent]
public sealed partial class RenameIdComponent : Component
{
    [DataField(required: true)]
    public string Value;

    [DataField]
    public string? NewIcon;
}
