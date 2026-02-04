// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Roles;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server._CD.JobSlotsConsole;

[RegisterComponent]
public sealed partial class JobSlotsConsoleComponent : Component
{
    /// <summary>
    /// The station this console is linked to. Set when the station is detected.
    /// </summary>
    [DataField]
    public EntityUid? Station;

    /// <summary>
    /// Jobs that cannot have their slots adjusted from this console.
    /// </summary>
    [DataField]
    public HashSet<ProtoId<JobPrototype>> Blacklist = [];

    /// <summary>
    /// Whether this console has debug features enabled, like toggling infinite slots.
    /// </summary>
    [DataField]
    public bool Debug;

    /// <summary>
    /// The sound to play if the player doesn't have access to change job slots.
    /// </summary>
    [DataField]
    public SoundSpecifier DenySound = new SoundPathSpecifier("/Audio/Effects/Cargo/buzz_sigh.ogg");
}
