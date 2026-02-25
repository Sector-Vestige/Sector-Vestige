// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Anzu <108382695+Anzuneth@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;

namespace Content.Server._Harmony.Uncloneable;

/// <summary>
/// This is used for the uncloneable trait.
/// </summary>
[RegisterComponent, Access(typeof(UncloneableSystem))]
public sealed partial class UncloneableComponent : Component;
