// SPDX-FileCopyrightText: 2026 EinsteinEngines contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 mqole <anactualpanacea@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Damage.Systems;
using Content.Shared.Damage;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Item.ItemToggle.Components;

/// <summary>
///   Handles changes to DamageOtherOnHitComponent when the item is toggled.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class ItemToggleDamageOtherOnHitComponent : Component
{
    /// <summary>
    ///   The stamina cost of throwing this entity when activated.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float? ActivatedStaminaCost = null;

    /// <summary>
    ///   The stamina cost of throwing this entity when deactivated.
    /// </summary>
    [DataField, AutoNetworkedField]
    public float? DeactivatedStaminaCost = null;

    /// <summary>
    ///     Damage done by this item when activated.
    /// </summary>
    [DataField, AutoNetworkedField]
    public DamageSpecifier? ActivatedDamage = null;

    /// <summary>
    ///     Damage done by this item when deactivated.
    /// </summary>
    [DataField, AutoNetworkedField]
    public DamageSpecifier? DeactivatedDamage = null;

    /// <summary>
    ///   The noise this item makes when hitting something with it on.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SoundSpecifier? ActivatedHitSound;

    /// <summary>
    ///   The noise this item makes when hitting something with it off.
    /// </summary>
    public SoundSpecifier? DeactivatedHitSound;

    /// <summary>
    ///  The noise this item makes when hitting something with it off and it does no damage.
    /// </summary>
    [DataField, AutoNetworkedField]
    public SoundSpecifier ActivatedNoDamageSound { get; set; } = new SoundCollectionSpecifier("WeakHit");

    /// <summary>
    ///   The noise this item makes when hitting something with it off and it does no damage.
    /// </summary>
    public SoundSpecifier? DeactivatedNoDamageSound;
}
