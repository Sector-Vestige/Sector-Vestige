// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;

namespace Content.Shared._CD.Silicons.StationAi;

[AutoGenerateComponentState]
[RegisterComponent, NetworkedComponent]

public sealed partial class StationAiShellBrainHolderComponent : Component
{
    [AutoNetworkedField]
    public EntityUid Brain { get; set; }
}
