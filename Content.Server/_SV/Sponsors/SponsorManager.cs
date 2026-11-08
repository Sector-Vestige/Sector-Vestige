using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Content.Shared._SV.CCVar;
using Content.Shared._SV.Sponsors;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Server._SV.Sponsors;

/// <summary>
/// Polls the entitlement store on a timer and holds an in-memory
/// NetUserId -> tierId map. Exposes an O(1) lookup for the OOC chat path.
/// Last-known-good on any fetch/parse failure; empty URL fully disables the feature.
/// </summary>
public sealed partial class SponsorManager
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IGameTiming _gameTiming = default!;
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private IHttpClientHolder _http = default!;
    [Dependency] private ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;

    private IEntitlementSource? _source;
    private string _url = string.Empty;
    private string _token = string.Empty;
    private TimeSpan _pollInterval = TimeSpan.FromSeconds(300);

    // Reference-swapped as a whole on each successful poll and read via a local
    // copy on the hot path; reference assignment is atomic, so no lock is needed.
    private IReadOnlyDictionary<NetUserId, string> _tiers = new Dictionary<NetUserId, string>();

    private TimeSpan _nextPoll;
    // Stale reads only delay the next poll by a tick, so no volatile/lock needed.
    private bool _polling;

    public void Initialize()
    {
        _sawmill = _logManager.GetSawmill("sponsors");

        _cfg.OnValueChanged(SVCCVars.SetSVPatreonStore, OnUrlChanged, true);
        _cfg.OnValueChanged(SVCCVars.SVPatreonToken, OnTokenChanged, true);
        _cfg.OnValueChanged(SVCCVars.SVPatreonPollInterval, OnIntervalChanged, true);
    }

    private void OnUrlChanged(string url)
    {
        _url = url;
        RebuildSource();
    }

    private void OnTokenChanged(string token)
    {
        _token = token;
        RebuildSource();
    }

    private void OnIntervalChanged(int seconds)
    {
        _pollInterval = TimeSpan.FromSeconds(seconds <= 0 ? 300 : seconds);
    }

    private void RebuildSource()
    {
        if (string.IsNullOrWhiteSpace(_url))
        {
            // Feature disabled: no source, no HTTP, no timer activity.
            _source = null;
            return;
        }

        _source = new RemoteEntitlementSource(_url, _token, _http.Client, _sawmill);
        _nextPoll = _gameTiming.RealTime; // poll on the next Update tick
    }

    public void Update()
    {
        if (_source == null || _polling)
            return;

        if (_gameTiming.RealTime < _nextPoll)
            return;

        _nextPoll = _gameTiming.RealTime + _pollInterval;
        Poll();
    }

    private async void Poll()
    {
        _polling = true;
        try
        {
            var source = _source;
            if (source == null)
                return;

            var result = await source.FetchAsync(CancellationToken.None);
            if (result != null)
                _tiers = result; // atomic swap; on null we keep last-known-good
        }
        catch (Exception e)
        {
            _sawmill.Error($"Unexpected error polling entitlements: {e}");
        }
        finally
        {
            _polling = false;
        }
    }

    /// <summary>
    /// Hot path. Dictionary lookup + prototype index lookup, nothing else.
    /// Returns true and the OOC colour hex (e.g. "#C27C0E") if the user is a patron.
    /// </summary>
    public bool TryGetOocColor(NetUserId user, [NotNullWhen(true)] out string? color)
    {
        color = null;

        var tiers = _tiers; // local copy: safe against a concurrent reference swap
        if (!tiers.TryGetValue(user, out var tierId))
            return false;

        if (!_proto.TryIndex<PatronTierPrototype>(tierId, out var proto))
            return false;

        color = proto.OocColor.ToHexNoAlpha();
        return true;
    }
}
