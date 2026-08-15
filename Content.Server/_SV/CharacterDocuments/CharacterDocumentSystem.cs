using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server._SV.CharacterDocuments;
using Content.Shared._SV.CharacterDocuments;
using Content.Shared._SV.CharacterDocuments.Components;
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.StationRecords;
using Robust.Shared.Player;
using Robust.Server.Player;
using Robust.Shared.Network;
using Content.Server._SV.CharacterDocuments.Consoles;
using Content.Shared._SV.CharacterDocuments.Consoles;
using Content.Server.CriminalRecords.Systems;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Events;
using Content.Server.Preferences.Managers;
using Content.Server.Station.Systems;
using Content.Shared._SV.CCVar;
using Content.Shared.StationRecords.Components;
using Content.Shared.StationRecords.Systems;
using Robust.Shared.Configuration;

namespace Content.Server._SV.CharacterDocuments;

public sealed partial class CharacterDocumentSystem : EntitySystem
{
    [Dependency] private IServerDbManager _db = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IServerPreferencesManager _prefs = default!;
    [Dependency] private IConfigurationManager _cfg = default!;

    /// <summary>
    ///     Authoritative in-round document store, keyed by ProfileId. Replaces the body
    ///     component as the runtime source of truth so documents survive gibbing. Cleared
    ///     on round restart.
    /// </summary>
    private readonly Dictionary<int, CharacterDocumentRecord> _records = new();

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnPlayerSpawn, after: [typeof(StationRecordsSystem)]);
        SubscribeLocalEvent<PlayerJoinedLobbyEvent>(OnPlayerJoinedLobby);
        SubscribeLocalEvent<RoundStartingEvent>(OnRoundStarting);
        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnCleanup);
    }

    private void OnCleanup(RoundRestartCleanupEvent ev)
    {
        // The store is per-round; drop everything so a new round starts fresh and stale
        // profiles from the previous round can't leak onto new consoles.
        _records.Clear();
    }

    /// <summary>
    ///     Looks up the in-round record for a profile. Returns false for the sentinel
    ///     ProfileId 0 (no selection) and for profiles that never loaded.
    /// </summary>
    public bool TryGetRecord(int profileId, [NotNullWhen(true)] out CharacterDocumentRecord? record)
    {
        record = null;
        return profileId != 0 && _records.TryGetValue(profileId, out record);
    }

    private void OnRoundStarting(RoundStartingEvent ev)
    {
        var days = _cfg.GetCVar(SVCCVars.CharacterDocumentBinRetentionDays);
        _ = PurgeExpiredBinAsync(TimeSpan.FromDays(Math.Max(0, days)));
    }

    private async Task PurgeExpiredBinAsync(TimeSpan retention)
    {
        var purged = await _db.PurgeExpiredSVCharacterDocumentsAsync(retention);
        if (purged > 0)
            Log.Info($"Purged {purged} expired character document(s) from the bin (retention {retention.TotalDays:0}d).");
    }

    /// <summary>
    ///     Safety net for in-round doc edits not making it to the client's lobby UI.
    ///     The normal path is <see cref="NotifyDocumentChangedAsync"/> →
    ///     <see cref="IServerPreferencesManager.RefreshPreferencesForUserAsync"/>, which
    ///     fires per-edit. If that push ever misses (target session was momentarily
    ///     detached at edit time, network blip between rounds, async exception swallowed
    ///     by an async-void handler), the DB still has the new docs but the client's
    ///     cached <c>Preferences</c> stays on pre-round state — so the lobby tab shows
    ///     stale docs until the player relogs.
    ///
    ///     Re-fetching on lobby entry guarantees the lobby is consistent with the DB
    ///     whenever a round ends (and on initial connect too, harmlessly redundant with
    ///     the one <see cref="ServerPreferencesManager.FinishLoad"/> already sends).
    /// </summary>
    private void OnPlayerJoinedLobby(PlayerJoinedLobbyEvent ev)
    {
        _ = _prefs.RefreshPreferencesForUserAsync(ev.PlayerSession);
    }

    private void OnPlayerSpawn(PlayerSpawnCompleteEvent args)
    {
        if (!HasComp<StationRecordsComponent>(args.Station))
        {
            Log.Warning("Station does not have StationRecordsComponent PlayerDocs will not work");
            return;
        }

        if (!HasComp<CharacterDocumentStationComponent>(args.Station))
        {
            AddComp<CharacterDocumentStationComponent>(args.Station);
        }

        var player = args.Mob;
        if (!HasComp<CharacterDocumentComponent>(player) && HasComp<HumanoidProfileComponent>(player))
        {
            AddComp<CharacterDocumentComponent>(player);
        }

        if (TryComp<CharacterDocumentComponent>(player, out var comp))
        {
            comp.ProfileName = args.Profile.Name;
            // Capture the account username while the session is guaranteed attached.
            // The save paths fall back to this if the player has since ghosted /
            // detached, instead of writing "Unknown" into SVProfile.PlayerName.
            if (_playerManager.TryGetSessionByEntity(player, out var session))
                comp.PlayerUsername = session.Name;
        }

        _ = LoadPlayerDocumentsAsync(player, args.Station, args.Profile.Name);
    }

    private async Task LoadPlayerDocumentsAsync(EntityUid uid, EntityUid station, string characterName)
    {
        _playerManager.TryGetSessionByEntity(uid, out var session);
        if (session == null) return;

        var netUserId = session.UserId;
        var playerName = session.Name;

        var prefs = await _db.GetPlayerPreferencesAsync(netUserId, CancellationToken.None);
        if (prefs == null)
        {
            Log.Warning($"Could not load preferences for player {playerName} ({characterName})");
            return;
        }

        var profile = prefs.Profiles.FirstOrDefault(p => p.CharacterName == characterName);
        if (profile == null)
        {
            Log.Debug($"Could not find profile '{characterName}' for player {playerName}");
            return;
        }

        // Link the live body to its profile so world-side systems (station roster,
        // parent-change) can find the record without going through the session.
        if (TryComp<CharacterDocumentComponent>(uid, out var docComp))
            docComp.ProfileId = profile.Id;

        var record = new CharacterDocumentRecord
        {
            ProfileId = profile.Id,
            Name = characterName,
            Username = docComp?.PlayerUsername is { Length: > 0 } captured ? captured : playerName,
            UserId = netUserId,
        };

        var result = await _db.GetSVCharacterDocumentsAsync(profile.Id);
        if (result != null)
        {
            foreach (var doc in result.Value.Documents)
            {
                record.Documents[doc.DocID] = new CharacterDocument
                {
                    DocID = doc.DocID,
                    DocType = doc.DocType,
                    DocTitle = doc.DocTitle,
                    DocAuthor = doc.DocAuthor,
                    DocLastEditedBy = doc.DocLastEditedBy,
                    DocDateLastEdited = doc.DocDateLastEdited,
                    DocContent = doc.DocContent,
                    DocStamps = CharacterDocumentDeserializer.DeserializeStamps(doc.DocStamps),
                    DeletedAt = doc.DeletedAt
                };
            }

            // SV: hydrate the General flavour block from the DB JSON column (piggybacked in tuple slot 1).
            if (result.Value.SerializedDocument != null)
            {
                try
                {
                    record.General =
                        System.Text.Json.JsonSerializer.Deserialize<CharacterDocumentGeneral>(result.Value.SerializedDocument)
                        ?? new CharacterDocumentGeneral();
                    record.General.EnsureValid();
                }
                catch
                {
                    record.General = new CharacterDocumentGeneral();
                }
            }
        }

        _records[profile.Id] = record;

        // ProfileId is now known, so the station roster can register this crew member and
        // open consoles can be refreshed. Membership registration lives in
        // CharacterDocumentStationSystem (faction filtering) and keys off the same ProfileId.
        RaiseLocalEvent(new CharacterDocumentProfileReadyEvent(uid, station, profile.Id, characterName));
    }

    public async Task AddDocument(int profileId, CharacterDocument characterDocument)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        // Rebalance the user-authored markup before it is persisted so stored content (and the
        // paper printed from it) can never carry the unmatched closing tags that crash the
        // rich-text renderer.
        characterDocument.DocContent = CharacterDocumentMarkup.Balance(characterDocument.DocContent);

        var session = GetSession(record);
        var playerName = ResolvePlayerName(record, session);

        var id = record.Documents.Count == 0 ? 1 : record.Documents.Keys.Max() + 1;
        characterDocument.DocID = id;
        record.Documents.Add(id, characterDocument);

        await PersistRecordAsync(record, playerName);
        await NotifyDocumentChangedAsync(session);
    }

    /// <summary>
    ///     Tells every open console to redraw AND pushes fresh preferences down to the
    ///     affected player so their lobby Documents tab + customization menu reflect
    ///     the change without requiring a reconnect.
    /// </summary>
    private async Task NotifyDocumentChangedAsync(ICommonSession? session)
    {
        RaiseLocalEvent(new CharacterDocumentEditedEvent());
        if (session != null)
            await _prefs.RefreshPreferencesForUserAsync(session);
    }

    public async Task DeleteDocument(int profileId, CharacterDocument characterDocument)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        var session = GetSession(record);
        var playerName = ResolvePlayerName(record, session);

        // Soft delete: the doc is hidden from normal listings but kept in the store with a
        // deletion timestamp so Central Command / admins can still review or restore it.
        // A background sweep purges it permanently once it outlives the retention window.
        // We keep it in the in-memory dict (carrying DeletedAt) so the replace-all save below
        // writes it back binned rather than dropping it.
        if (record.Documents.TryGetValue(characterDocument.DocID, out var stored))
            stored.DeletedAt = DateTime.UtcNow;
        else
            return;

        await PersistRecordAsync(record, playerName);
        await NotifyDocumentChangedAsync(session);
    }

    /// <summary>
    ///     Restores a binned (soft-deleted) document back into normal view by clearing its
    ///     deletion timestamp. Only reachable from privileged flows (Central Command console,
    ///     admin EUI) — see the access check in the console handler.
    /// </summary>
    public async Task RestoreDocument(int profileId, int docId)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        if (!record.Documents.TryGetValue(docId, out var stored) || stored.DeletedAt == null)
            return;

        var session = GetSession(record);
        var playerName = ResolvePlayerName(record, session);

        stored.DeletedAt = null;

        await PersistRecordAsync(record, playerName);
        await NotifyDocumentChangedAsync(session);
    }

    /// <summary>
    ///     Permanently deletes a single binned (soft-deleted) document, bypassing the
    ///     retention window. Only binned docs may be purged — a live document must be
    ///     sent to the bin first. Irreversible. Only reachable from privileged flows
    ///     (Central Command console, admin EUI) — see the access check in the handlers.
    /// </summary>
    public async Task PurgeDocument(int profileId, int docId)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        // Guard: only binned docs can be permanently deleted, matching the UI which only
        // offers the control from the bin view. Refuse to purge a live document.
        if (!record.Documents.TryGetValue(docId, out var stored) || stored.DeletedAt == null)
            return;

        var session = GetSession(record);
        var playerName = ResolvePlayerName(record, session);

        record.Documents.Remove(docId);

        await PersistRecordAsync(record, playerName);
        await NotifyDocumentChangedAsync(session);
    }

    /// <summary>
    ///     Permanently deletes every binned (soft-deleted) document for the player — the
    ///     "empty the recycling bin" action. Live documents are left untouched. Irreversible.
    ///     Only reachable from privileged flows — see the access check in the handlers.
    /// </summary>
    public async Task EmptyBin(int profileId)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        var binnedIds = record.Documents
            .Where(kv => kv.Value.DeletedAt != null)
            .Select(kv => kv.Key)
            .ToList();

        if (binnedIds.Count == 0)
            return;

        var session = GetSession(record);
        var playerName = ResolvePlayerName(record, session);

        foreach (var id in binnedIds)
            record.Documents.Remove(id);

        await PersistRecordAsync(record, playerName);
        await NotifyDocumentChangedAsync(session);
    }

    public async Task UpdateDocument(int profileId, CharacterDocument characterDocument)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        // Rebalance the user-authored markup before it is persisted (see AddDocument).
        characterDocument.DocContent = CharacterDocumentMarkup.Balance(characterDocument.DocContent);

        var session = GetSession(record);
        var playerName = ResolvePlayerName(record, session);

        record.Documents[characterDocument.DocID] = characterDocument;

        await PersistRecordAsync(record, playerName);
        await NotifyDocumentChangedAsync(session);
    }

    /// <summary>
    ///     Replaces an existing record's live document set from an out-of-band edit (the admin
    ///     documents browser) and broadcasts a console refresh. No-op if the profile isn't
    ///     loaded in-round. Persistence is handled by the caller.
    /// </summary>
    public void ReplaceRecordDocuments(int profileId, IEnumerable<CharacterDocument> documents)
    {
        if (!TryGetRecord(profileId, out var record))
            return;

        record.Documents.Clear();
        foreach (var doc in documents)
            record.Documents[doc.DocID] = doc;

        RaiseLocalEvent(new CharacterDocumentEditedEvent());
    }

    /// <summary>
    ///     Writes the record's full in-memory document set back to the DB. The DB save is a
    ///     replace-all, so removing entries from the record before calling this permanently
    ///     drops them. Persistence remains keyed by ProfileId — the DB format is unchanged.
    /// </summary>
    private async Task PersistRecordAsync(CharacterDocumentRecord record, string playerName)
    {
        var dbDocs = record.Documents.Values.Select(doc => new SVModel.CharacterDocument
        {
            DocTitle = doc.DocTitle,
            DocAuthor = doc.DocAuthor,
            DocLastEditedBy = doc.DocLastEditedBy,
            DocContent = doc.DocContent,
            DocDateLastEdited = doc.DocDateLastEdited,
            DocStamps = CharacterDocumentSerializer.SerializeStamp(doc.DocStamps),
            DocType = doc.DocType,
            DeletedAt = doc.DeletedAt,
            ProfileId = record.ProfileId
        }).ToList();

        await _db.SaveSVCharacterDocumentsAsync(record.ProfileId, playerName, record.Name, dbDocs);
    }

    /// <summary>
    ///     Resolves the owning player's session by their account id. Unlike the old
    ///     entity-based lookup this still works after the body is gone (ghosted / gibbed);
    ///     returns null if they're fully disconnected, in which case the DB write still
    ///     happens and only the live prefs push is skipped.
    /// </summary>
    private ICommonSession? GetSession(CharacterDocumentRecord record)
    {
        _playerManager.TryGetSessionById(record.UserId, out var session);
        return session;
    }

    /// <summary>
    ///     Resolves the account username to persist as <c>SVProfile.PlayerName</c>.
    ///     Prefers the username captured on the record at spawn (stable for the
    ///     whole round, regardless of ghosting). Falls back to the live session if
    ///     that's somehow empty, and finally to "Unknown".
    /// </summary>
    private static string ResolvePlayerName(CharacterDocumentRecord record, ICommonSession? session)
    {
        if (!string.IsNullOrEmpty(record.Username))
            return record.Username;
        return session?.Name ?? "Unknown";
    }
}

public sealed class CharacterDocumentEditedEvent : EntityEventArgs;

/// <summary>
///     Raised once a player's documents have finished loading from the DB and their record
///     exists in the store (so ProfileId is known). The station roster system listens for
///     this to register crew membership by ProfileId and refresh open consoles.
/// </summary>
public sealed class CharacterDocumentProfileReadyEvent : EntityEventArgs
{
    public readonly EntityUid Mob;
    public readonly EntityUid Station;
    public readonly int ProfileId;
    public readonly string Name;

    public CharacterDocumentProfileReadyEvent(EntityUid mob, EntityUid station, int profileId, string name)
    {
        Mob = mob;
        Station = station;
        ProfileId = profileId;
        Name = name;
    }
}
