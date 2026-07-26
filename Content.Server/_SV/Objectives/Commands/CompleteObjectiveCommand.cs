using System.Collections.Generic;
using Content.Server.Administration;
using Content.Server.Objectives.Components;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Content.Shared.Objectives.Systems;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server._SV.Objectives.Commands;

/// <summary>
/// Marks one of a player's objectives complete. Only works on objectives that have no condition
/// tracking them, which is every custom objective plus a handful of code-driven stock ones.
/// </summary>
[AdminCommand(AdminFlags.Admin)]
public sealed partial class CompleteObjectiveCommand : LocalizedEntityCommands
{
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private CustomObjectiveSystem _custom = default!;
    [Dependency] private SharedMindSystem _mind = default!;
    [Dependency] private SharedObjectivesSystem _objectives = default!;

    public override string Command => "customobjectivecomplete";

    private string ObjectiveName(EntityUid uid) => EntityManager.GetComponent<MetaDataComponent>(uid).EntityName;

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("cmd-completeobjective-invalid-args"));
            return;
        }

        if (!_players.TryGetSessionByUsername(args[0], out var session))
        {
            shell.WriteError(Loc.GetString("cmd-completeobjective-player-not-found"));
            return;
        }

        if (!_mind.TryGetMind(session, out _, out var mind))
        {
            shell.WriteError(Loc.GetString("cmd-completeobjective-mind-not-found"));
            return;
        }

        if (!int.TryParse(args[1], out var index))
        {
            shell.WriteError(Loc.GetString("cmd-completeobjective-invalid-index", ("index", args[1])));
            return;
        }

        if (index < 0 || index >= mind.Objectives.Count)
        {
            shell.WriteError(Loc.GetString("cmd-completeobjective-index-out-of-range",
                ("index", index),
                ("count", mind.Objectives.Count)));
            return;
        }

        // separated from the range check so the two failures don't look the same to the admin
        if (!_custom.TrySetCompleted(mind, index))
        {
            shell.WriteError(Loc.GetString("cmd-completeobjective-not-manual",
                ("objective", ObjectiveName(mind.Objectives[index]))));
            return;
        }

        shell.WriteLine(Loc.GetString("cmd-completeobjective-success",
            ("index", index),
            ("objective", ObjectiveName(mind.Objectives[index]))));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            return CompletionResult.FromHintOptions(CompletionHelper.SessionNames(),
                LocalizationManager.GetString("shell-argument-username-hint"));
        }

        if (args.Length != 2)
            return CompletionResult.Empty;

        if (!_players.TryGetSessionByUsername(args[0], out var session))
            return CompletionResult.Empty;

        if (!_mind.TryGetMind(session, out var mindId, out var mind))
            return CompletionResult.Empty;

        // only offer the objectives this command can actually act on
        var options = new List<CompletionOption>();
        for (var i = 0; i < mind.Objectives.Count; i++)
        {
            var objective = mind.Objectives[i];
            if (!EntityManager.HasComponent<CodeConditionComponent>(objective))
                continue;

            var info = _objectives.GetInfo(objective, mindId, mind);
            var hint = info == null
                ? ObjectiveName(objective)
                : Loc.GetString("cmd-completeobjective-objective-hint",
                    ("objective", info.Value.Title),
                    ("progress", (int) (info.Value.Progress * 100)));

            options.Add(new CompletionOption(i.ToString(), hint));
        }

        return CompletionResult.FromOptions(options);
    }
}
