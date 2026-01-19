// SPDX-FileCopyrightText: 2025 Wizards Den contributors
// SPDX-FileCopyrightText: 2025 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 DrSmugleaf <10968691+DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Players.JobWhitelist;
using Content.Shared.Roles;
using Content.Shared._SV.JobWhitelist; // SV changes - Job whitelist groups
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Players.JobWhitelist;

public sealed class JobWhitelistManager : IPostInjectInit
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IPrototypeManager _prototypes = default!;
    [Dependency] private readonly UserDbDataManager _userDb = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    private readonly Dictionary<NetUserId, HashSet<string>> _whitelists = new();
    private ISawmill _sawmill = default!;
    // SV changes start - Job whitelist groups
    private readonly Dictionary<NetUserId, HashSet<string>> _groupWhitelists = new();
    // SV changes end

    public void Initialize()
    {
        _net.RegisterNetMessage<MsgJobWhitelist>();
    }

    private async Task LoadData(ICommonSession session, CancellationToken cancel)
    {
        var whitelists = await _db.GetJobWhitelists(session.UserId.UserId, cancel);
        // SV changes start - Job whitelist groups
        var groups = await _db.GetJobWhitelistGroups(session.UserId.UserId, cancel);
        // SV changes end
        cancel.ThrowIfCancellationRequested();
        _whitelists[session.UserId] = whitelists.ToHashSet();
        // SV changes start - Job whitelist groups
        _groupWhitelists[session.UserId] = groups.ToHashSet();
        // SV changes end
    }

    private void FinishLoad(ICommonSession session)
    {
        SendJobWhitelist(session);
    }

    private void ClientDisconnected(ICommonSession session)
    {
        _whitelists.Remove(session.UserId);
        // SV changes start - Job whitelist groups
        _groupWhitelists.Remove(session.UserId);
        // SV changes end
    }

    public async void AddWhitelist(NetUserId player, ProtoId<JobPrototype> job)
    {
        if (_whitelists.TryGetValue(player, out var whitelists))
            whitelists.Add(job);

        await _db.AddJobWhitelist(player, job);

        if (_player.TryGetSessionById(player, out var session))
            SendJobWhitelist(session);
    }

    /// <summary>
    /// Returns false if role whitelist is required but the player does not have it.
    /// </summary>
    public bool IsAllowed(ICommonSession session, ProtoId<JobPrototype> job)
    {
        if (!_config.GetCVar(CCVars.GameRoleWhitelist))
            return true;

        if (!_prototypes.Resolve(job, out var jobPrototype) ||
            !jobPrototype.Whitelisted)
        {
            return true;
        }

        return IsWhitelisted(session.UserId, job);
    }

    public bool IsWhitelisted(NetUserId player, ProtoId<JobPrototype> job)
    {
        if (!_whitelists.TryGetValue(player, out var whitelists))
        {
            _sawmill.Error("Unable to check if player {Player} is whitelisted for {Job}. Stack trace:\\n{StackTrace}",
                player,
                job,
                Environment.StackTrace);
            return false;
        }

        // Check direct job whitelist
        if (whitelists.Contains(job))
            return true;

        // SV changes start - Job whitelist groups
        // Check if player has any groups that include this job
        if (_groupWhitelists.TryGetValue(player, out var groups))
        {
            foreach (var groupId in groups)
            {
                if (_prototypes.TryIndex<JobWhitelistGroupPrototype>(groupId, out var groupProto) &&
                    groupProto.Jobs.Contains(job))
                {
                    return true;
                }
            }
        }
        // SV changes end

        return false;
    }

    public async void RemoveWhitelist(NetUserId player, ProtoId<JobPrototype> job)
    {
        _whitelists.GetValueOrDefault(player)?.Remove(job);
        await _db.RemoveJobWhitelist(player, job);

        if (_player.TryGetSessionById(new NetUserId(player), out var session))
            SendJobWhitelist(session);
    }

    // SV changes start - Job whitelist groups
    public async void AddGroup(NetUserId player, string groupId)
    {
        if (!_groupWhitelists.TryGetValue(player, out var groups))
        {
            groups = new HashSet<string>();
            _groupWhitelists[player] = groups;
        }

        groups.Add(groupId);

        await _db.AddJobWhitelistGroup(player.UserId, groupId);

        if (_player.TryGetSessionById(player, out var session))
            SendJobWhitelist(session);
    }

    public async void RemoveGroup(NetUserId player, string groupId)
    {
        _groupWhitelists.GetValueOrDefault(player)?.Remove(groupId);
        await _db.RemoveJobWhitelistGroup(player.UserId, groupId);

        if (_player.TryGetSessionById(new NetUserId(player), out var session))
            SendJobWhitelist(session);
    }

    public IEnumerable<string> GetPlayerGroups(NetUserId player)
    {
        return _groupWhitelists.GetValueOrDefault(player) ?? Enumerable.Empty<string>();
    }
    // SV changes end

    public void SendJobWhitelist(ICommonSession player)
    {
        var whitelist = new HashSet<string>(_whitelists.GetValueOrDefault(player.UserId) ?? new HashSet<string>());

        // SV changes start - Job whitelist groups
        // Add jobs from all groups the player is in
        if (_groupWhitelists.TryGetValue(player.UserId, out var groups))
        {
            foreach (var groupId in groups)
            {
                if (_prototypes.TryIndex<JobWhitelistGroupPrototype>(groupId, out var groupProto))
                {
                    foreach (var job in groupProto.Jobs)
                    {
                        whitelist.Add(job.Id);
                    }
                }
            }
        }
        // SV changes end

        var msg = new MsgJobWhitelist
        {
            Whitelist = whitelist
        };

        _net.ServerSendMessage(msg, player.Channel);
    }

    void IPostInjectInit.PostInject()
    {
        _userDb.AddOnLoadPlayer(LoadData);
        _userDb.AddOnFinishLoad(FinishLoad);
        _userDb.AddOnPlayerDisconnect(ClientDisconnected);
        _sawmill = _logManager.GetSawmill("job_whitelist");
    }
}
