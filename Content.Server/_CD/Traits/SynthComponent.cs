// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2025 Contributors of the _CD upstream project
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <vinjeerik@gmail.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 qu4drivium <aaronholiver@outlook.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Chat.TypingIndicator;
using Robust.Shared.Prototypes;
using Content.Shared.Chemistry.Components;

namespace Content.Server._CD.Traits;

/// <summary>
/// Set players' blood to coolant, and is used to notify them of ion storms
/// </summary>
[RegisterComponent, Access(typeof(SynthSystem))]
public sealed partial class SynthComponent : Component
{
    /// <summary>
    /// The chance that the synth is alerted of an ion storm
    /// </summary>
    [DataField]
    public float AlertChance = 0.3f;

    [DataField]
    public Solution BloodReferenceSolution = new([new("SynthBlood", 300)]);

    /// <summary>
    /// The typing indicator prototype to use for synths
    /// </summary>
    [DataField]
    public ProtoId<TypingIndicatorPrototype> TypingIndicatorPrototype = "robot";
}
