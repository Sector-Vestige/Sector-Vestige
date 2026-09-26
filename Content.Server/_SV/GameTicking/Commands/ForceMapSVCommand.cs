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
    public sealed partial class ForceMapSVCommand : ForceMapCommand
    {
        [Dependency] private IConfigurationManager _configurationManager = default!;
        [Dependency] private IGameMapManager _gameMapManager = default!;
        [Dependency] private IPrototypeManager _prototypeManager = default!;

        public override string Description => Loc.GetString("cmd-forcemapsv-desc");
        public override string Help => Loc.GetString("cmd-forcemapsv-help");

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
                shell.WriteLine(Loc.GetString("cmd-forcemapsv-map-not-found", ("map", name)));
                return;
            }

            _configurationManager.SetCVar(CCVars.GameMap, name);

            if (string.IsNullOrEmpty(name))
                shell.WriteLine(Loc.GetString("cmd-forcemapsv-cleared"));
            else
                shell.WriteLine(Loc.GetString("cmd-forcemapsv-success", ("map", name)));
        }

        public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            if (args.Length != 1)
                return CompletionResult.Empty;

            var options = _prototypeManager.EnumeratePrototypes<GameMapPoolPrototype>()
                .Where(pool => pool.Selectable)
                .SelectMany(pool => pool.Maps)
                .Distinct()
                .Select(map => new CompletionOption(map,
                    _prototypeManager.TryIndex<GameMapPrototype>(map, out var proto) ? proto.MapName : null))
                .OrderBy(p => p.Value);

            return CompletionResult.FromHintOptions(options, Loc.GetString("cmd-forcemapsv-hint"));
        }
    }
}
