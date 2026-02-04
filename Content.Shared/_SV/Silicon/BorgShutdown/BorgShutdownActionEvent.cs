// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.Silicon.BorgShutdown;

/// <summary>
/// SV: Action event for toggling borg shutdown state.
/// </summary>
public sealed partial class BorgShutdownActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class BorgShutdownDoAfterEvent : SimpleDoAfterEvent
{
}
