// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.EUI;
using Content.Shared.Preferences;

namespace Content.Server._CD.Admin;

public sealed class EventPreferenceEui : BaseEui
{
    private readonly HumanoidCharacterProfile _playerPref;
    private readonly LocatedPlayerData _targetPlayer;

    public EventPreferenceEui(LocatedPlayerData player, HumanoidCharacterProfile pref)
    {
        IoCManager.InjectDependencies(this);
        _targetPlayer = player;
        _playerPref = pref;
    }

    public async void SetState()
    {
        StateDirty();
    }
}
