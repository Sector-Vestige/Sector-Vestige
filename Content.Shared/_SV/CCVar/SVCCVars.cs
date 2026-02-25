// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._SV.CCVar;

/// <summary>
/// Sector Vestige specific CVars.
/// </summary>
[CVarDefs]
public sealed class SVCCVars : CVars
{
    /// <summary>
    /// Whether or not job whitelist groups are enabled.
    /// When disabled, group whitelists are ignored and only individual job whitelists apply.
    /// </summary>
    public static readonly CVarDef<bool>
        GameGroupWhitelist = CVarDef.Create("sv.group_whitelist", true, CVar.SERVER | CVar.REPLICATED);
}
