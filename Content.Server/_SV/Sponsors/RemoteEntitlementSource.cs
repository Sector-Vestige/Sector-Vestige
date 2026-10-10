using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Server._SV.Sponsors;

/// <summary>
/// Reads the entitlement store's read feed over HTTP and parses it into a
/// GUID -> tierId map. On ANY failure returns null so the caller keeps
/// last-known-good.
/// </summary>
public sealed class RemoteEntitlementSource : IEntitlementSource
{
    private readonly string _url;
    private readonly string _token;
    private readonly HttpClient _http;
    private readonly ISawmill _sawmill;

    public RemoteEntitlementSource(string url, string token, HttpClient http, ISawmill sawmill)
    {
        _url = url;
        _token = token;
        _http = http;
        _sawmill = sawmill;
    }

    public async Task<IReadOnlyDictionary<NetUserId, string>?> FetchAsync(CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, _url);
            if (!string.IsNullOrWhiteSpace(_token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            using var response = await _http.SendAsync(request, ct);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _sawmill.Warning($"Entitlement store returned {(int) response.StatusCode}; keeping last-known-good.");
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            if (!TryParse(json, out var map))
            {
                _sawmill.Warning("Failed to parse entitlement store response; keeping last-known-good.");
                return null;
            }

            return map;
        }
        catch (Exception e)
        {
            _sawmill.Warning($"Entitlement store unreachable ({e.Message}); keeping last-known-good.");
            return null;
        }
    }

    /// <summary>
    /// Parses the store response body. Separated from HTTP so it is unit-testable.
    /// Returns false on malformed JSON or a missing/non-object "entitlements".
    /// An empty "entitlements" object is valid and yields an empty map.
    /// Individual unparseable GUID keys or empty values are skipped, not fatal.
    /// </summary>
    public static bool TryParse(string json, out Dictionary<NetUserId, string> map)
    {
        map = new Dictionary<NetUserId, string>();
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return false;

            if (!doc.RootElement.TryGetProperty("entitlements", out var entitlements)
                || entitlements.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            foreach (var entry in entitlements.EnumerateObject())
            {
                if (!Guid.TryParse(entry.Name, out var guid))
                    continue;

                if (entry.Value.ValueKind != JsonValueKind.String)
                    continue;

                var tierId = entry.Value.GetString();
                if (string.IsNullOrWhiteSpace(tierId))
                    continue;

                map[new NetUserId(guid)] = tierId;
            }

            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
