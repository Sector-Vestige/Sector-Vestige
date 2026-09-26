using Content.Shared._SV.CharacterDocuments.Components;
using Content.Shared.Security;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.CharacterDocuments.Consoles;

[Serializable, NetSerializable]
public sealed class CharacterDocumentConsoleState : BoundUserInterfaceState
{
    /// <summary>Crew roster shown by the console, keyed by stable ProfileId → character name.</summary>
    public Dictionary<int, string> PlayerList = new();
    /// <summary>Currently-selected player's ProfileId, or null if none selected.</summary>
    public int? SelectedPlayer;
    public Dictionary<int, CharacterDocument>? SelectedPlayerDocuments;
    public CharacterDocument? SelectedDocument;
    public bool PaperInserted;
    public DocumentType DocumentType;
    /// <summary>
    /// Extra types this console covers in addition to <see cref="DocumentType"/>.
    /// Empty for single-type consoles; non-empty for the Central Command terminal.
    /// </summary>
    public List<DocumentType> AdditionalDocumentTypes = new();
    public SecurityStatus SecurityStatus;
    public string? SecurityReason;
    /// <summary>
    /// Selected player's fingerprint, read from their station record. Null when no player is
    /// selected, no station record matches their name, or the record never captured one.
    /// Security consoles only; other document types leave this null.
    /// </summary>
    public string? SelectedPlayerFingerprint;
    /// <summary>
    /// Selected player's General flavour block (allergies, height, etc). Null if no player selected.
    /// Console UIs render relevant fields based on the active tab (or primary type for single-type consoles).
    /// </summary>
    public CharacterDocumentGeneral? SelectedPlayerGeneral;

    /// <summary>
    /// Whether this console may view and restore binned (soft-deleted) documents.
    /// True only for Central Command terminals. Stamped by the server on every state push;
    /// when true, <see cref="SelectedPlayerDocuments"/> also contains binned docs (those with
    /// a non-null <c>DeletedAt</c>) so the client can offer a recycling-bin view.
    /// </summary>
    public bool CanAccessBin;

    public CharacterDocumentConsoleState(Dictionary<int, string> playerlist, int? selectedplayer,
        Dictionary<int, CharacterDocument>? selectedplayerdocuments, CharacterDocument? selecteddocument, bool paperinserted,
        DocumentType documentType = DocumentType.Employment,
        SecurityStatus securityStatus = SecurityStatus.None, string? securityReason = null,
        List<DocumentType>? additionalDocumentTypes = null,
        CharacterDocumentGeneral? selectedPlayerGeneral = null,
        string? selectedPlayerFingerprint = null)
    {
        PlayerList = playerlist;
        SelectedPlayer = selectedplayer;
        SelectedPlayerDocuments = selectedplayerdocuments;
        SelectedDocument = selecteddocument;
        PaperInserted = paperinserted;
        DocumentType = documentType;
        SecurityStatus = securityStatus;
        SecurityReason = securityReason;
        AdditionalDocumentTypes = additionalDocumentTypes ?? new List<DocumentType>();
        SelectedPlayerGeneral = selectedPlayerGeneral;
        SelectedPlayerFingerprint = selectedPlayerFingerprint;
    }
}
