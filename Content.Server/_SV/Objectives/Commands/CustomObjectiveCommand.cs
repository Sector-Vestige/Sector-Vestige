using System.Collections.Generic;
using System.Linq;
using Content.Server.Administration;
using Content.Shared._SV.Objectives;
using Content.Shared.Administration;
using Content.Shared.Mind;
using Content.Shared.Objectives.Prototypes;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Server._SV.Objectives.Commands;

/// <summary>
/// Hands a player a freeform objective authored on the spot. Complete it later with completeobjective.
/// </summary>
[AdminCommand(AdminFlags.Admin)]
public sealed partial class CustomObjectiveCommand : LocalizedEntityCommands
{
    [Dependency] private IPlayerManager _players = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private CustomObjectiveSystem _custom = default!;
    [Dependency] private SharedMindSystem _mind = default!;

    public override string Command => "customobjectivecreate";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length is < 3 or > 5)
        {
            shell.WriteError(Loc.GetString("cmd-customobjective-invalid-args"));
            return;
        }

        if (!_players.TryGetSessionByUsername(args[0], out var session))
        {
            shell.WriteError(Loc.GetString("cmd-customobjective-player-not-found"));
            return;
        }

        if (!_mind.TryGetMind(session, out var mindId, out var mind))
        {
            shell.WriteError(Loc.GetString("cmd-customobjective-mind-not-found"));
            return;
        }

        var title = args[1];
        if (string.IsNullOrWhiteSpace(title))
        {
            shell.WriteError(Loc.GetString("cmd-customobjective-empty-title"));
            return;
        }

        // resolved here rather than in the system so a typo is an error instead of a silent default
        ProtoId<CustomObjectiveIconPrototype>? icon = null;
        if (args.Length >= 4)
        {
            if (!_proto.HasIndex<CustomObjectiveIconPrototype>(args[3]))
            {
                shell.WriteError(Loc.GetString("cmd-customobjective-icon-not-found", ("icon", args[3])));
                return;
            }

            icon = args[3];
        }

        ProtoId<ObjectiveIssuerPrototype>? issuer = null;
        if (args.Length == 5)
        {
            if (!_proto.HasIndex<ObjectiveIssuerPrototype>(args[4]))
            {
                shell.WriteError(Loc.GetString("cmd-customobjective-issuer-not-found", ("issuer", args[4])));
                return;
            }

            issuer = args[4];
        }

        if (!_custom.TryAddCustomObjective(mindId, mind, title, args[2], icon, issuer, out _))
        {
            shell.WriteError(Loc.GetString("cmd-customobjective-adding-failed"));
            return;
        }

        // the index is what completeobjective takes, so hand it back rather than making them list
        shell.WriteLine(Loc.GetString("cmd-customobjective-success",
            ("index", mind.Objectives.Count - 1),
            ("player", session.Name)));
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        return args.Length switch
        {
            1 => CompletionResult.FromHintOptions(CompletionHelper.SessionNames(),
                LocalizationManager.GetString("shell-argument-username-hint")),
            2 => CompletionResult.FromHint(Loc.GetString("cmd-customobjective-title-hint")),
            3 => CompletionResult.FromHint(Loc.GetString("cmd-customobjective-description-hint")),
            4 => CompletionResult.FromHintOptions(IconOptions(), Loc.GetString("cmd-customobjective-icon-hint")),
            5 => CompletionResult.FromHintOptions(IssuerOptions(), Loc.GetString("cmd-customobjective-issuer-hint")),
            _ => CompletionResult.Empty,
        };
    }

    /// <summary>
    /// CompletionHelper.PrototypeIDs gives bare ids with no hints, which says nothing about what
    /// an icon actually looks like, so spell out the sprite each one points at.
    /// </summary>
    private IEnumerable<CompletionOption> IconOptions()
    {
        foreach (var proto in _proto.EnumeratePrototypes<CustomObjectiveIconPrototype>().OrderBy(p => p.ID))
        {
            var hint = proto.Icon switch
            {
                SpriteSpecifier.Rsi rsi => $"{rsi.RsiPath} [{rsi.RsiState}]",
                SpriteSpecifier.Texture texture => texture.TexturePath.ToString(),
                _ => proto.ID,
            };

            yield return new CompletionOption(proto.ID, hint);
        }
    }

    /// <summary>
    /// Issuer ids are not what the player sees, so hint with the name it renders as.
    /// The localized names carry colour markup that would show up literally in the console.
    /// </summary>
    private IEnumerable<CompletionOption> IssuerOptions()
    {
        foreach (var proto in _proto.EnumeratePrototypes<ObjectiveIssuerPrototype>().OrderBy(p => p.ID))
        {
            yield return new CompletionOption(proto.ID, FormattedMessage.RemoveMarkupPermissive(proto.LocalizedName));
        }
    }
}
