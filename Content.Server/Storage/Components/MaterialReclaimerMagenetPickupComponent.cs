// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 s3ptemb3rr (GitHub)
// SPDX-FileCopyrightText: 2026 september <rdamico2204@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Server.Storage.Components;

/// <summary>
/// Applies an ongoing pickup area around the attached entity.
/// </summary>
[RegisterComponent]
public sealed partial class MaterialReclaimerMagnetPickupComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite), DataField("nextScan")]
    public TimeSpan NextScan = TimeSpan.Zero;

    [ViewVariables(VVAccess.ReadWrite), DataField("range")]
    public float Range = 1f;

    /// <summary>
    /// Frontier - Is the magnet currently enabled?
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("magnetEnabled")]
    public bool MagnetEnabled = false;
}
