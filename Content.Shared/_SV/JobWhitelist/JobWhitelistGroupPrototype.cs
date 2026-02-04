// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.JobWhitelist;

/// <summary>
/// Defines a group of jobs that can be whitelisted together.
/// For example, a "trusted" group might include all head roles.
/// </summary>
[Prototype("jobWhitelistGroup")]
public sealed partial class JobWhitelistGroupPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Human-readable name for this whitelist group.
    /// </summary>
    [DataField(required: true)]
    public string Name = string.Empty;

    /// <summary>
    /// Description of what this group grants access to.
    /// </summary>
    [DataField]
    public string Description = string.Empty;

    /// <summary>
    /// List of jobs that this group grants access to.
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<JobPrototype>> Jobs = new();
}
