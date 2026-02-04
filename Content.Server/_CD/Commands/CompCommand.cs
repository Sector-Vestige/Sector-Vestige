// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Administration;
using Content.Server.Preferences.Managers;
using Content.Shared.Administration;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Toolshed;

namespace Content.Server._CD.Admin.Commands;

[ToolshedCommand, AdminCommand(AdminFlags.Admin)]
public sealed class PrefCommand : ToolshedCommand
{
    [Dependency] private readonly IServerPreferencesManager _pref = default!;

    [CommandImplementation("has")]
    public bool Has(
        [PipedArgument] ICommonSession session,
        [CommandArgument] string proto)
    {
        var protoId = (ProtoId<AntagPrototype>) proto;
        var pref = (HumanoidCharacterProfile)_pref.GetPreferences(session.UserId).SelectedCharacter;
        foreach (var antag in pref.AntagPreferences)
        {
            if (antag.Equals(protoId))
                return true;
        }

        return false;
    }
}
