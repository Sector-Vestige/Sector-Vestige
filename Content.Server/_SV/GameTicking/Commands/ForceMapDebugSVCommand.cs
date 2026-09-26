using System.Linq;
using Content.Server.Administration;
using Content.Server.GameTicking.Commands;
using Content.Server.Maps;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Content.Shared.Maps;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Prototypes;

namespace Content.Server._SV.GameTicking.Commands
{
    [AdminCommand(AdminFlags.Round)]
    public sealed partial class ForceMapDebugSVCommand : ForceMapCommand
    {
        [Dependency] private IConfigurationManager _configurationManager = default!;
        [Dependency] private IGameMapManager _gameMapManager = default!;
        [Dependency] private IPrototypeManager _prototypeManager = default!;

        public override string Command => "forcemapdebug";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            if (args.Length != 1)
            {
                shell.WriteLine(Loc.GetString("shell-need-exactly-one-argument"));
                return;
            }

            var name = args[0];

            // An empty string clears the forced map
            if (!string.IsNullOrEmpty(name) && !_gameMapManager.CheckMapExists(name))
            {
                shell.WriteLine(Loc.GetString("cmd-forcemapdebug-map-not-found", ("map", name)));
                return;
            }

            _configurationManager.SetCVar(CCVars.GameMap, name);

            if (string.IsNullOrEmpty(name))
                shell.WriteLine(Loc.GetString("cmd-forcemapdebug-cleared"));
            else
                shell.WriteLine(Loc.GetString("cmd-forcemapdebug-success", ("map", name)));
        }

        public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length != 1)
                return CompletionResult.Empty;

            var options = _prototypeManager
                .EnumeratePrototypes<GameMapPrototype>()
                .Select(p => new CompletionOption(p.ID, p.MapName))
                .OrderBy(p => p.Value);

            return CompletionResult.FromHintOptions(options, Loc.GetString("cmd-forcemapdebug-hint"));
        }
    }
}
