// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 NakataRin <45946146+NakataRin@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._Harmony.Clothing.Gloves.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared._Harmony.Clothing.Gloves.Components;

/// <summary>
/// Component for mantis blade gloves that can extend and retract deadly blades.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedMantisGlovesSystem))]
public sealed partial class MantisGlovesComponent : Component
{
    [DataField, AutoNetworkedField]
    public string? ActivatedName = "mantis-gloves-name-active";

    [DataField, AutoNetworkedField]
    public string? ActivatedDescription = "mantis-gloves-desc-active";

    [DataField, AutoNetworkedField]
    public string? ActivatedPopUp = "mantis-gloves-activated";

    [DataField, AutoNetworkedField]
    public string? DeactivatedPopUp = "mantis-gloves-deactivated";
}
