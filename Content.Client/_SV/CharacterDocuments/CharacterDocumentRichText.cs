using Content.Client.RichText;
using Content.Shared._SV.CharacterDocuments;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._SV.CharacterDocuments;

/// <summary>
///     Rendering helpers for the user-authored markup stored in a character document.
/// </summary>
public static class CharacterDocumentRichText
{
    /// <summary>
    ///     Safely renders a document's content into a <see cref="RichTextLabel"/>.
    /// </summary>
    /// <remarks>
    ///     Document content is untrusted player text. Assigning it raw to <see cref="RichTextLabel.Text"/>
    ///     (1) allows every markup tag, and (2) crashes the renderer when the tags are unbalanced:
    ///     two unmatched <c>[/color]</c>/<c>[/font]</c> tags underflow the draw-context stack. This helper
    ///     rebalances the markup via <see cref="CharacterDocumentMarkup.BuildBalancedMessage"/> and limits
    ///     it to the same player-safe formatting tags used for in-game paper.
    /// </remarks>
    public static void SetDocumentContent(this RichTextLabel label, string? content)
    {
        var message = CharacterDocumentMarkup.BuildBalancedMessage(content);
        label.SetMessage(message, UserFormattableTags.BaseAllowedTags);
    }
}
