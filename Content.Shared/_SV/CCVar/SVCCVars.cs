using Robust.Shared;
using Robust.Shared.Configuration;

namespace Content.Shared._SV.CCVar;

/// <summary>
/// Sector Vestige specific CVars.
/// </summary>
[CVarDefs]
public sealed class SVCCVars : CVars
{
    /// <summary>
    /// Whether or not job whitelist groups are enabled.
    /// When disabled, group whitelists are ignored and only individual job whitelists apply.
    /// </summary>
    public static readonly CVarDef<bool>
        GameGroupWhitelist = CVarDef.Create("sv.group_whitelist", true, CVar.SERVER | CVar.REPLICATED);

    /// <summary>
    /// How many days a soft-deleted ("binned") character document is retained before it is
    /// permanently purged. During this window the document is invisible to normal viewers
    /// but can still be reviewed / restored by Central Command consoles and admins.
    /// Set to 0 (or less) to purge binned documents immediately on the next sweep.
    /// </summary>
    public static readonly CVarDef<int>
        CharacterDocumentBinRetentionDays = CVarDef.Create("sv.character_documents.bin_retention_days", 30, CVar.SERVERONLY);

    /// <summary>
    /// A toggle to show SV's patrons yes or no
    /// </summary>
    public static readonly CVarDef<bool>
        ShowSVOocPatreonColor = CVarDef.Create("sv.show_ooc_patreon_color", true, CVar.ARCHIVE | CVar.REPLICATED | CVar.CLIENT);

    /// <summary>
    /// CVAR to set the link to the SV_Patreon_Store
    /// </summary>
    public static readonly CVarDef<string>
        SetSVPatreonStore = CVarDef.Create("sv.patreon_store", string.Empty, CVar.SERVER);

    /// <summary>
    /// Bearer token for reading the SV Patreon entitlement store.
    /// Secret: set via server config, never committed.
    /// </summary>
    public static readonly CVarDef<string>
        SVPatreonToken = CVarDef.Create("sv.patreon_token", string.Empty, CVar.SERVERONLY);

    /// <summary>
    /// How often (in seconds) to poll the entitlement store. Default 300.
    /// </summary>
    public static readonly CVarDef<int>
        SVPatreonPollInterval = CVarDef.Create("sv.patreon_poll_interval", 300, CVar.SERVERONLY);

    /// <summary>
    /// How long, in seconds, mapvotesv lasts.
    /// </summary>
    public static readonly CVarDef<int>
        MapVoteDuration = CVarDef.Create("sv.mapvote.runoff_duration", 150, CVar.SERVERONLY);

    /// <summary>
    /// Percentage added to every cargo product price. 50 = prices are 1.5x.
    /// </summary>
    public static readonly CVarDef<int>
        CargoMarkupPercent = CVarDef.Create("sv.cargo.markup_percent", 0, CVar.SERVER | CVar.REPLICATED);
}
