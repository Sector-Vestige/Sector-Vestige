using Content.Server.Administration;
using Content.Shared._SV.CCVar;
using Content.Shared.Administration;
using Robust.Shared.Configuration;
using Robust.Shared.Console;

namespace Content.Server._SV.GameTicking.Commands;

[AdminCommand(AdminFlags.Round)]
public sealed partial class CargoMarkupSVCommand : LocalizedEntityCommands
{
    [Dependency] private IConfigurationManager _cfg = default!;

    public override string Command => "setcargomarkup";
    public override string Description => "Increases the prices for cargo by a certain persentage (basevalue = 0)";
    public override string Help => "Usage: setcargomarkup 50, then the price increases to 150%";
    public const string Highlightcolor = "#30d5c8";

    public override void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteError(Loc.GetString("shell-need-exactly-one-argument"));
            return;
        }

        if (!int.TryParse(args[0], out var percent))
        {
            shell.WriteError(Loc.GetString("shell-argument-must-be-number"));
            return;
        }

        _cfg.SetCVar(SVCCVars.CargoMarkupPercent, percent);
        var newvalue = _cfg.GetCVar(SVCCVars.CargoMarkupPercent);
        shell.WriteMarkup(Loc.GetString("sv-shell-confirm-return-value",
            ("thing", "markup"), ("highlightcolor", Highlightcolor), ("value", newvalue)));

    }
}
