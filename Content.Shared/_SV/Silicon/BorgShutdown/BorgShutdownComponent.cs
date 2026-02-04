// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Shared._SV.Silicon.BorgShutdown;

/// <summary>
/// SV: Component that allows borgs to shutdown (drain battery to 0) and restore battery charge.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class BorgShutdownComponent : Component
{
    /// <summary>
    /// Whether the borg is currently shut down.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool IsShutdown;

    /// <summary>
    /// The stored battery charge before shutdown, used to restore on wake up.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float StoredCharge;
}
