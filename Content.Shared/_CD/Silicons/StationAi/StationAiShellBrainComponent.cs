// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._CD.Silicons.StationAi;

/// <summary>
/// Given to a brain to allow an AI to take it over.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class StationAiShellBrainComponent : Component
{
    /// <summary>
    /// If this brain is currently possessed
    /// </summary>
    [DataField]
    public bool Active;

    /// <summary>
    /// The core which owns and is possessing this brain
    /// </summary>
    [DataField]
    public EntityUid? ActiveCore;

    [DataField]
    public EntityUid? ContainingShell;
}
