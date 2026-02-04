// SPDX-FileCopyrightText: 2026 Umbra contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <131422822+OnyxTheBrave@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Server._Umbra.Power.Components;

[RegisterComponent]
public sealed partial class ElectricalOverloadComponent : Component
{
    [DataField]
    public string ExplosionOnOverload = "Default";

    [ViewVariables]
    public DateTime ExplodeAt = DateTime.MaxValue;

    [ViewVariables]
    public DateTime NextBuzz = DateTime.MaxValue;
}
