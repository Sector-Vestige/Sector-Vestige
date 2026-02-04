// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <vinjeerik@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

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
