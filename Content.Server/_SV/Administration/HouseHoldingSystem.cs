using Content.Shared._SV.CCVar;
using Content.Shared.GameTicking;
using Robust.Shared.Configuration;

namespace Content.Server._SV.Administration;

/// <summary>
/// This system is purely used for householding tasks.
/// For example setting values back to their default values so they dont overflow into the next round.
/// </summary>
public sealed partial class HouseHoldingSystem : EntitySystem
{
    [Dependency] private IConfigurationManager _cfg = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestartCleanup);
    }

    public void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
    {
        var cargoMarkupPercent = _cfg.GetCVar(SVCCVars.CargoMarkupPercent);

        if (cargoMarkupPercent != SVCCVars.CargoMarkupPercent.DefaultValue)
        {
            _cfg.SetCVar(SVCCVars.CargoMarkupPercent, SVCCVars.CargoMarkupPercent.DefaultValue);
        }
    }
}
