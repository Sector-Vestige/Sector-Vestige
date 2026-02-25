// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 Contributors of the _CD upstream project
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <vinjeerik@gmail.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;

namespace Content.Server._CD.StationEvents.Components;

/// <summary>
/// Gamerule component to notify a random synth player when started.
/// </summary>
[RegisterComponent]
public sealed partial class SynthStormRuleComponent : Component
{
    [DataField]
    public SoundSpecifier? SynthStormSound = new SoundPathSpecifier("/Audio/Misc/cryo_warning.ogg");
}
