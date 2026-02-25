// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Administration;
using Content.Server.Database;
using Content.Server.Players.JobWhitelist;
using Content.Shared.Administration;
using Content.Shared._SV.JobWhitelist;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server._SV.Jobwhitelist;

[AdminCommand(AdminFlags.Ban)]
public sealed class JobWhitelistAddGroupCommand : LocalizedCommands
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly JobWhitelistManager _jobWhitelist = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;

    public override string Command => "jobwhitelistaddgroup";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 2),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var player = args[0].Trim();
        var groupId = args[1].Trim();

        if (!_prototypes.TryIndex<JobWhitelistGroupPrototype>(groupId, out var group))
        {
            shell.WriteError(Loc.GetString("cmd-jobwhitelist-group-does-not-exist", ("group", groupId)));
            shell.WriteLine(Help);
            return;
        }

        var data = await _playerLocator.LookupIdByNameAsync(player);
        if (data != null)
        {
            var guid = data.UserId;
            var isWhitelisted = await _db.IsJobWhitelistGroupWhitelisted(guid.UserId, groupId);
            if (isWhitelisted)
            {
                shell.WriteLine(Loc.GetString("cmd-jobwhitelistaddgroup-already-whitelisted",
                    ("player", player),
                    ("group", group.Name)));
                return;
            }

            _jobWhitelist.AddGroup(guid, groupId);
            shell.WriteLine(Loc.GetString("cmd-jobwhitelistaddgroup-added",
                ("player", player),
                ("group", group.Name)));

            return;
        }

        shell.WriteError(Loc.GetString("parse-session-fail", ("username", player)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(players: _players),
                Loc.GetString("cmd-jobwhitelistaddgroup-arg-player")),
            2 => CompletionResult.FromHintOptions(
                _prototypes.EnumeratePrototypes<JobWhitelistGroupPrototype>().Select(p => p.ID),
                Loc.GetString("cmd-jobwhitelistaddgroup-arg-group")),
            _ => CompletionResult.Empty
        };
    }
}

[AdminCommand(AdminFlags.Ban)]
public sealed class JobWhitelistRemoveGroupCommand : LocalizedCommands
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly JobWhitelistManager _jobWhitelist = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IPlayerManager _players = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;

    public override string Command => "jobwhitelistremovegroup";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("shell-wrong-arguments-number-need-specific",
                ("properAmount", 2),
                ("currentAmount", args.Length)));
            shell.WriteLine(Help);
            return;
        }

        var player = args[0].Trim();
        var groupId = args[1].Trim();

        if (!_prototypes.TryIndex<JobWhitelistGroupPrototype>(groupId, out var group))
        {
            shell.WriteError(Loc.GetString("cmd-jobwhitelist-group-does-not-exist", ("group", groupId)));
            shell.WriteLine(Help);
            return;
        }

        var data = await _playerLocator.LookupIdByNameAsync(player);
        if (data != null)
        {
            var guid = data.UserId;
            var isWhitelisted = await _db.IsJobWhitelistGroupWhitelisted(guid.UserId, groupId);
            if (!isWhitelisted)
            {
                shell.WriteLine(Loc.GetString("cmd-jobwhitelistremovegroup-not-whitelisted",
                    ("player", player),
                    ("group", group.Name)));
                return;
            }

            _jobWhitelist.RemoveGroup(guid, groupId);
            shell.WriteLine(Loc.GetString("cmd-jobwhitelistremovegroup-removed",
                ("player", player),
                ("group", group.Name)));

            return;
        }

        shell.WriteError(Loc.GetString("parse-session-fail", ("username", player)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(players: _players),
                Loc.GetString("cmd-jobwhitelistremovegroup-arg-player")),
            2 => CompletionResult.FromHintOptions(
                _prototypes.EnumeratePrototypes<JobWhitelistGroupPrototype>().Select(p => p.ID),
                Loc.GetString("cmd-jobwhitelistremovegroup-arg-group")),
            _ => CompletionResult.Empty
        };
    }
}
