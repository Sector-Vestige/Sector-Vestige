using Content.Shared._SV.CharacterDocuments;

namespace Content.Client.Lobby.UI;

// SV: bridges the SV documents lobby tab into the profile editor's IsDirty flow.
public sealed partial class HumanoidProfileEditor
{
    private void UpdateProfileSVDocuments(List<CharacterDocument> docs)
    {
        if (Profile is null)
            return;
        Profile = Profile.WithSVCharacterDocuments(docs);
        IsDirty = true;
    }

    private void UpdateProfileSVDocumentGeneral(CharacterDocumentGeneral general)
    {
        if (Profile is null)
            return;
        Profile = Profile.WithSVCharacterDocumentGeneral(general);
        IsDirty = true;
    }
}
