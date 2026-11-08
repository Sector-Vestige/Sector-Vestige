using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Server._SV.Sponsors;

/// <summary>
/// Source of patron entitlements: a map of account GUID (NetUserId) to internal tier id.
/// </summary>
public interface IEntitlementSource
{
    /// <summary>
    /// Fetches the full entitlement map. Returns null on ANY fetch/parse failure,
    /// signalling the caller to keep its last-known-good map.
    /// </summary>
    Task<IReadOnlyDictionary<NetUserId, string>?> FetchAsync(CancellationToken ct);
}
