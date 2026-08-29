// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Administration.Logs;
using Content.Server.Database;
using Content.Server.EUI;
using Content.Server.Preferences.Managers;
using Content.Shared._SV.CharacterDocuments;
using Content.Shared._SV.CharacterDocuments.Admin;
using Content.Shared._SV.CharacterDocuments.Components;
using Content.Shared.Database;
using Content.Shared.Eui;
using Robust.Server.Player;

namespace Content.Server._SV.CharacterDocuments.Admin;

public sealed partial class AdminCharacterDocumentsEui : BaseEui
{
    [Dependency] private IServerDbManager _db = default!;
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private IPlayerManager _playerManager = default!;
    [Dependency] private IServerPreferencesManager _prefs = default!;
    [Dependency] private IAdminLogManager _adminLogger = default!;

    private List<AdminSVProfileEntry> _profiles = new();

    public AdminCharacterDocumentsEui()
    {
        IoCManager.InjectDependencies(this);
    }

    public override EuiStateBase GetNewState()
    {
        return new AdminCharacterDocumentsEuiState { Profiles = _profiles };
    }

    public override void Opened()
    {
        base.Opened();
        _ = ReloadAsync();
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        switch (msg)
        {
            case AdminSVRefreshMsg:
                _ = ReloadAsync();
                break;
            case AdminSVDocumentEditMsg edit:
                _ = ApplyEditAsync(edit.ProfileId, edit.Document);
                break;
            case AdminSVDocumentDeleteMsg del:
                _ = ApplyDeleteAsync(del.ProfileId, del.DocId);
                break;
            case AdminSVDocumentRestoreMsg restore:
                _ = ApplyRestoreAsync(restore.ProfileId, restore.DocId);
                break;
            case AdminSVDocumentPurgeMsg purge:
                _ = ApplyPurgeAsync(purge.ProfileId, purge.DocId);
                break;
            case AdminSVDocumentEmptyBinMsg empty:
                _ = ApplyEmptyBinAsync(empty.ProfileId);
                break;
            case AdminSVDocumentCreateMsg create:
                _ = ApplyCreateAsync(create);
                break;
        }
    }

    private async Task ReloadAsync()
    {
        var dbProfiles = await _db.GetAllSVCharacterDocumentsAsync(CancellationToken.None);

        _profiles = dbProfiles
            .OrderBy(p => p.PlayerName)
            .ThenBy(p => p.CharacterName)
            .Select(p => new AdminSVProfileEntry
            {
                ProfileId = p.ProfileId,
                UserId = p.Profile?.Preference?.UserId ?? Guid.Empty,
                PlayerName = p.PlayerName,
                CharacterName = p.CharacterName,
                Documents = p.CharacterDocuments
                    .Select(d => new CharacterDocument
                    {
                        DocID = d.DocID,
                        DocType = d.DocType,
                        DocTitle = d.DocTitle,
                        DocAuthor = d.DocAuthor,
                        DocLastEditedBy = d.DocLastEditedBy,
                        DocDateLastEdited = d.DocDateLastEdited,
                        DocContent = d.DocContent,
                        DocStamps = CharacterDocumentDeserializer.DeserializeStamps(d.DocStamps),
                        DeletedAt = d.DeletedAt,
                    })
                    .OrderBy(d => d.DocTitle)
                    .ToList(),
            })
            .ToList();

        StateDirty();
    }

    private const int TitleMaxLen = 256;
    // 16-06-2025 - 18:26: Noone is gonna hit 16k -VI
    public const int ContentMaxLen = 16384;

    private async Task ApplyEditAsync(int profileId, CharacterDocument incoming)
    {
        var entry = _profiles.FirstOrDefault(p => p.ProfileId == profileId);
        if (entry == null)
            return;

        var idx = entry.Documents.FindIndex(d => d.DocID == incoming.DocID);
        if (idx < 0)
            return;

        // Mutate in-memory only as a scratchpad; the canonical state comes from PersistAsync's reload.
        var existing = entry.Documents[idx];
        existing.DocTitle = Clamp(incoming.DocTitle, TitleMaxLen);
        existing.DocContent = Clamp(incoming.DocContent, ContentMaxLen);
        existing.DocStamps = incoming.DocStamps ?? new List<CharacterDocumentStamp>();
        existing.DocLastEditedBy = "Central Command";
        existing.DocDateLastEdited = DateTime.Now.AddYears(200);

        await PersistAsync(profileId, entry);
    }

    private async Task ApplyDeleteAsync(int profileId, int docId)
    {
        var entry = _profiles.FirstOrDefault(p => p.ProfileId == profileId);
        if (entry == null)
            return;

        // Soft delete: send the doc to the retention bin rather than removing it outright,
        // matching the in-game console behaviour. A background sweep purges it permanently
        // once it outlives the retention window.
        var doc = entry.Documents.FirstOrDefault(d => d.DocID == docId);
        if (doc == null || doc.DeletedAt != null)
            return;

        doc.DeletedAt = DateTime.UtcNow;

        _adminLogger.Add(LogType.CharacterDocument, LogImpact.Medium,
            $"Admin {Player:player} sent character document '{doc.DocTitle}' (#{doc.DocID}) of {entry.CharacterName} ({entry.PlayerName}) to the recycling bin via the admin documents browser");

        await PersistAsync(profileId, entry);
    }

    private async Task ApplyRestoreAsync(int profileId, int docId)
    {
        var entry = _profiles.FirstOrDefault(p => p.ProfileId == profileId);
        if (entry == null)
            return;

        var doc = entry.Documents.FirstOrDefault(d => d.DocID == docId);
        if (doc is not { DeletedAt: not null })
            return;

        doc.DeletedAt = null;

        await PersistAsync(profileId, entry);
    }

    private async Task ApplyPurgeAsync(int profileId, int docId)
    {
        var entry = _profiles.FirstOrDefault(p => p.ProfileId == profileId);
        if (entry == null)
            return;

        // Only binned docs can be permanently deleted, matching the in-game console and the
        // admin UI (which only offers Purge on a binned doc). Refuse to purge a live document.
        var doc = entry.Documents.FirstOrDefault(d => d.DocID == docId);
        if (doc is not { DeletedAt: not null })
            return;

        _adminLogger.Add(LogType.CharacterDocument, LogImpact.High,
            $"Admin {Player:player} permanently deleted binned character document '{doc.DocTitle}' (#{doc.DocID}) of {entry.CharacterName} ({entry.PlayerName}) via the admin documents browser");

        // The DB save is a replace-all, so dropping it from the list here purges it for good.
        entry.Documents.Remove(doc);

        await PersistAsync(profileId, entry);
    }

    private async Task ApplyEmptyBinAsync(int profileId)
    {
        var entry = _profiles.FirstOrDefault(p => p.ProfileId == profileId);
        if (entry == null)
            return;

        var removed = entry.Documents.RemoveAll(d => d.DeletedAt != null);
        if (removed == 0)
            return;

        _adminLogger.Add(LogType.CharacterDocument, LogImpact.High,
            $"Admin {Player:player} emptied the recycling bin ({removed} document(s)) of {entry.CharacterName} ({entry.PlayerName}) via the admin documents browser");

        await PersistAsync(profileId, entry);
    }

    private async Task ApplyCreateAsync(AdminSVDocumentCreateMsg msg)
    {
        var entry = _profiles.FirstOrDefault(p => p.ProfileId == msg.ProfileId);
        if (entry == null)
            return;

        // Reject unknown document types from a malicious or stale client.
        if (!Enum.IsDefined(typeof(DocumentType), msg.DocType))
            return;

        // The DocID here is throwaway — the DB autoincrements on insert, and the
        // canonical id arrives via PersistAsync's reload before anyone sees state.
        var throwawayId = entry.Documents.Count == 0 ? 1 : entry.Documents.Max(d => d.DocID) + 1;

        entry.Documents.Add(new CharacterDocument
        {
            DocID = throwawayId,
            DocType = msg.DocType,
            DocTitle = Clamp(string.IsNullOrWhiteSpace(msg.Title) ? "Untitled" : msg.Title, TitleMaxLen),
            DocAuthor = "Central Command",
            DocLastEditedBy = "Central Command",
            DocDateLastEdited = DateTime.Now.AddYears(200),
            DocContent = Clamp(msg.Content ?? string.Empty, ContentMaxLen),
            DocStamps = msg.Stamps ?? new List<CharacterDocumentStamp>(),
        });

        await PersistAsync(msg.ProfileId, entry);
    }

    /// <summary>
    ///     Single canonical save path. Persists the entry to the DB, reloads the
    ///     admin cache so canonical autoincrement DocIDs replace any throwaway
    ///     in-memory ones, then syncs the live player session with the canonical
    ///     state and finally pushes state to admin clients. Nobody — neither admin
    ///     UI nor live console — ever sees the throwaway IDs.
    /// </summary>
    private async Task PersistAsync(int profileId, AdminSVProfileEntry entry)
    {
        var dbDocs = entry.Documents.Select(d => new SVModel.CharacterDocument
        {
            DocTitle = d.DocTitle,
            DocAuthor = d.DocAuthor,
            DocLastEditedBy = d.DocLastEditedBy,
            DocContent = d.DocContent,
            DocDateLastEdited = d.DocDateLastEdited,
            DocStamps = CharacterDocumentSerializer.SerializeStamp(d.DocStamps),
            DocType = d.DocType,
            DeletedAt = d.DeletedAt,
            ProfileId = entry.ProfileId,
        }).ToList();

        await _db.SaveSVCharacterDocumentsAsync(entry.ProfileId, entry.PlayerName, entry.CharacterName, dbDocs);

        await ReloadAsync();

        var fresh = _profiles.FirstOrDefault(p => p.ProfileId == profileId);
        if (fresh != null)
            SyncLiveSession(fresh);

        // Push fresh preferences to the affected player (if connected) so their
        // lobby tab + customization menu reflect the admin edit immediately.
        // Prefer the reloaded entry's UserId — `entry` here is the pre-reload
        // copy and may have an empty UserId on rows that pre-date this fix.
        await RefreshPlayerPrefsAsync(fresh ?? entry);
    }

    /// <summary>
    ///     Pushes fresh preferences to the affected player so their open lobby UI
    ///     re-renders the new docs. Looks up the session by UserId rather than by name —
    ///     names can drift in casing or whitespace between save and login (especially
    ///     for OAuth-style usernames), UserId never does.
    /// </summary>
    private async Task RefreshPlayerPrefsAsync(AdminSVProfileEntry entry)
    {
        if (entry.UserId == Guid.Empty)
            return;

        foreach (var session in _playerManager.Sessions)
        {
            if (session.UserId.UserId == entry.UserId)
            {
                await _prefs.RefreshPreferencesForUserAsync(session);
                return;
            }
        }
    }

    private static string Clamp(string? s, int maxLen)
    {
        if (string.IsNullOrEmpty(s)) return string.Empty;
        return s.Length > maxLen ? s[..maxLen] : s;
    }

    /// <summary>
    ///     If the target character's documents are loaded in-round, replace the record's
    ///     document set in the store and broadcast a refresh so any open consoles re-render.
    ///     Without this, admins editing online players' docs would only show up after the
    ///     player reconnects. No-op if the profile isn't loaded in-round.
    /// </summary>
    private void SyncLiveSession(AdminSVProfileEntry entry)
    {
        _entMan.System<CharacterDocumentSystem>().ReplaceRecordDocuments(entry.ProfileId, entry.Documents);
    }
}
