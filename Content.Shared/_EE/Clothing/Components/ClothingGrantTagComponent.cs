// SPDX-FileCopyrightText: 2026 EinsteinEngines contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 Lachryphage (GitHub)
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Lachryphage <lachri.lachryphage@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared._EE.Clothing;

[RegisterComponent]
public sealed partial class ClothingGrantTagComponent : Component
{
    [DataField("tag", required: true), ViewVariables(VVAccess.ReadWrite)]
    public string Tag = "";

    [ViewVariables(VVAccess.ReadWrite)]
    public bool IsActive = false;
}
