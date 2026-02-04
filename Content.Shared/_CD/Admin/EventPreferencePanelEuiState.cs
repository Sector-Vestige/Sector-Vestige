// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Eui;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._CD.Admin;

[Serializable, NetSerializable]
public sealed class EventPreferencePanelEuiState(
    string username,
    HumanoidCharacterProfile preferences,
    List<ProtoId<AntagPrototype>> visibleAntagPrototypes) : EuiStateBase
{
    public readonly string Username = username;
    public readonly HumanoidCharacterProfile Preferences = preferences;
    public readonly List<ProtoId<AntagPrototype>> VisibleAntagPrototypes = visibleAntagPrototypes;
}
