// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._CD.Spawners;

/// <summary>
/// Makes every entity with a job spawn at the point(s) with _jobId, whether latejoining or doing so immediately.
/// </summary>
[RegisterComponent]
public sealed partial class ArrivalsSpawnPointComponent : Component
{
    /// <summary>
    /// The jobId of the job(s) that should spawn at this point. If null, a (general) spawn point to be used as a fallback if no respective job spawners exist.
    /// </summary>
    [DataField("jobs")]
    public List<string> JobIds = new();

    /// <summary>
    /// The jobId of the job(s) that should ignore spawners.
    /// </summary>
    [DataField]
    public List<string> IgnoredJobs = new();
}
