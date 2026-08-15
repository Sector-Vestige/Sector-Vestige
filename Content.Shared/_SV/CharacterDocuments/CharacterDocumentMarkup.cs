// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using Robust.Shared.Utility;

namespace Content.Shared._SV.CharacterDocuments;

/// <summary>
///     Helpers for safely handling the user-authored markup stored in a character document's
///     <c>DocContent</c>.
/// </summary>
public static class CharacterDocumentMarkup
{
    /// <summary>
    ///     Parses <paramref name="content"/> permissively and returns a <see cref="FormattedMessage"/>
    ///     whose tags are guaranteed to be balanced: closing tags that never had a matching opening
    ///     tag are dropped and any tags left open at the end are closed. Safe to feed straight into
    ///     a rich-text control.
    /// </summary>
    /// <remarks>
    ///     Authors routinely overlap tags rather than nesting them properly — <c>[head]a[bold]b[/head]c[/bold]</c>
    ///     is a perfectly clear intent even though it isn't well-formed. Such a closing tag is honoured
    ///     where it was written by closing the tags opened after it, closing it, then reopening those
    ///     tags, so only the tag the author closed stops applying.
    /// </remarks>
    public static FormattedMessage BuildBalancedMessage(string? content)
    {
        FormattedMessage parsed;
        try
        {
            parsed = FormattedMessage.FromMarkupPermissive(content ?? string.Empty);
        }
        catch
        {
            // Even the permissive parser can throw on pathological input. Fall back to rendering
            // the raw content as plain, un-parsed text — never crash on a document.
            var plain = new FormattedMessage();
            plain.AddText(content ?? string.Empty);
            return plain;
        }

        var result = new FormattedMessage();
        // Opening nodes currently open, innermost last. Mirrors the FormattedMessage's own
        // internal open-node stack so result.Pop() always closes the matching tag, and keeps the
        // original nodes around so a tag can be reopened with its attributes intact.
        var open = new List<MarkupNode>();

        foreach (var node in parsed)
        {
            if (node.Name == null)
            {
                result.AddText(node.Value.StringValue ?? string.Empty);
                continue;
            }

            if (!node.Closing)
            {
                // Re-add the original opening node verbatim so colours/attributes are preserved.
                result.PushTag(node);
                open.Add(node);
                continue;
            }

            // A closer with nothing to close would underflow the renderer's draw stack, so drop it.
            var index = open.FindLastIndex(openNode => openNode.Name == node.Name);
            if (index < 0)
                continue;

            // Unwind down to the tag being closed. Everything opened after it is still wanted by
            // the author, so note it before closing and put it straight back afterwards.
            var reopen = open.GetRange(index + 1, open.Count - index - 1);
            for (var i = index; i < open.Count; i++)
                result.Pop();

            open.RemoveRange(index, open.Count - index);

            foreach (var reopened in reopen)
            {
                result.PushTag(reopened);
                open.Add(reopened);
            }
        }

        // Close anything the user left open, innermost first.
        for (var i = open.Count - 1; i >= 0; i--)
            result.Pop();

        return result;
    }

    /// <summary>
    ///     Returns <paramref name="content"/> with its markup tags rebalanced as a markup string.
    ///     Use this on the persistence/print paths where a plain string is required; use
    ///     <see cref="BuildBalancedMessage"/> directly when feeding a rich-text control.
    /// </summary>
    public static string Balance(string? content)
    {
        return BuildBalancedMessage(content).ToMarkup();
    }
}
