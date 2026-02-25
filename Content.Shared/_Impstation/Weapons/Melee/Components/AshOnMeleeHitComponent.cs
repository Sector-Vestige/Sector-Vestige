// SPDX-FileCopyrightText: 2026 Impstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 beck <163376292+widgetbeck@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Impstation.Weapons.Melee;

/// <summary>
/// Ashes the target on melee hits.
/// </summary>
[RegisterComponent]
public sealed partial class AshOnMeleeHitComponent : Component
{
    [DataField("spawn")]
    public EntProtoId AshPrototype = "Ash";

    /// <summary>
    /// The popup that appears upon ashing.
    /// </summary>
    [DataField]
    public string Popup = "ash-on-melee-generic";

    /// <summary>
    /// The sound played upon ashing.
    /// </summary>
    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/_EE/Supermatter/supermatter.ogg");

    /// <summary>
    /// Whether the entity deletes itself after ashing something.
    /// </summary>
    [DataField]
    public bool SingleUse = true;
}
