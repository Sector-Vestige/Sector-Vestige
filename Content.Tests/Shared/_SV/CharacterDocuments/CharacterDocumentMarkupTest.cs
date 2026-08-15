// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.Linq;
using Content.Shared._SV.CharacterDocuments;
using NUnit.Framework;
using Robust.Shared.Utility;

namespace Content.Tests.Shared._SV.CharacterDocuments;

/// <summary>
///     Tests for <see cref="CharacterDocumentMarkup"/>, the helper that rebalances the
///     user-authored markup in a character document so it can't crash the rich-text renderer.
///
///     Background: document content is arbitrary user text rendered as markup. The renderer
///     (RobustToolbox <c>RichTextEntry.Update</c>/<c>Draw</c>) seeds the colour/font draw stack
///     with a single default entry and then pops it unconditionally for every closing
///     <c>[/color]</c>/<c>[/font]</c> tag (see <c>ColorTag.PopDrawContext</c>). Two unmatched
///     closing tags therefore pop an empty stack and throw <see cref="System.InvalidOperationException"/>
///     on every measure/draw pass, permanently breaking the document console for that document.
/// </summary>
[Parallelizable]
[TestFixture]
[TestOf(typeof(CharacterDocumentMarkup))]
public sealed class CharacterDocumentMarkupTest
{
    /// <summary>
    ///     Faithfully mirrors the renderer's colour/font stack handling: each stack starts with
    ///     the one default entry the renderer pushes, an opening tag pushes, a closing tag pops,
    ///     and a pop on an empty stack is the <see cref="System.InvalidOperationException"/> that
    ///     crashes the console. Returns true if rendering this message would underflow.
    /// </summary>
    private static bool RenderWouldThrow(FormattedMessage message)
    {
        // Only colour and font use the draw-context stacks that can underflow.
        var depth = new Dictionary<string, int> { ["color"] = 1, ["font"] = 1 };

        foreach (var node in message)
        {
            if (node.Name is not ("color" or "font"))
                continue;

            if (!node.Closing)
            {
                depth[node.Name]++;
                continue;
            }

            if (depth[node.Name] == 0)
                return true; // pop on empty stack -> InvalidOperationException

            depth[node.Name]--;
        }

        return false;
    }

    private static bool RenderWouldThrow(string markup)
        => RenderWouldThrow(FormattedMessage.FromMarkupPermissive(markup));

    /// <summary>
    ///     Describes which tags are actually in effect over each run of visible text, which is the
    ///     behaviour a document author sees. Rendered as <c>'text'&lt;tag,tag&gt;</c> per run so a
    ///     failure message reads like the document itself.
    /// </summary>
    private static string Describe(FormattedMessage message)
    {
        var runs = new List<string>();
        var open = new List<string>();

        foreach (var node in message)
        {
            if (node.Name == null)
            {
                runs.Add($"'{node.Value.StringValue}'<{string.Join(",", open)}>");
                continue;
            }

            if (!node.Closing)
            {
                open.Add(node.Name);
                continue;
            }

            var index = open.LastIndexOf(node.Name);
            if (index >= 0)
                open.RemoveAt(index);
        }

        return string.Join(" | ", runs);
    }

    private static string Describe(string markup)
        => Describe(CharacterDocumentMarkup.BuildBalancedMessage(markup));

    [Test]
    public void OuterClosingTagEndsFormattingWhereItWasWritten()
    {
        // Issue #380: the tag opened first had its closer dropped and silently re-emitted at the
        // end of the document, so the heading bled over every following paragraph.
        Assert.That(Describe("[head=2][bold]Title[/head]body[/bold]"),
            Is.EqualTo("'Title'<head,bold> | 'body'<bold>"));
    }

    [Test]
    public void OverlappingTagsKeepTheStillOpenTagAlive()
    {
        // The inner tag has to be closed to close the outer one, then reopened so the author's
        // remaining text keeps the formatting they asked for.
        Assert.That(Describe("[bold][color=red]X[/bold]Y[/color]"),
            Is.EqualTo("'X'<bold,color> | 'Y'<color>"));
    }

    [Test]
    public void ReopenedTagsAreClosedAtTheEndOfTheDocument()
    {
        Assert.That(Describe("[bold][color=red]X[/bold]Y"),
            Is.EqualTo("'X'<bold,color> | 'Y'<color>"));
    }

    [Test]
    public void StrayClosingTagForANeverOpenedTagIsStillDropped()
    {
        Assert.That(Describe("[/head]text"), Is.EqualTo("'text'<>"));
    }

    [Test]
    public void EveryClosingTagAppliesWhenTagsAreClosedInOpeningOrder()
    {
        // The exact shape reported in issue #380: closing in the order the tags were opened meant
        // only the innermost tag ([color]) actually closed, and [head]/[bold] ran on to the end.
        Assert.That(Describe("[head=2][bold][color=red]text[/head][/bold][/color]after"),
            Is.EqualTo("'text'<head,bold,color> | 'after'<>"));
    }

    [Test]
    public void SavedMarkupKeepsTheClosingTagWhereTheAuthorPutIt()
    {
        // Documents are rebalanced on the save path, so this is the markup the author gets back
        // when they reopen the document. Issue #380: the [/head] used to reappear at the very end.
        Assert.That(CharacterDocumentMarkup.Balance("[head=2][bold]Title[/head]body[/bold]"),
            Is.EqualTo("[head=2][bold]Title[/bold][/head][bold]body[/bold]"));
    }

    [Test]
    public void RebalancedOverlappingMarkupIsIdempotent()
    {
        const string raw = "[head=2][bold]Title[/head]body[/bold]";
        var once = CharacterDocumentMarkup.Balance(raw);
        var twice = CharacterDocumentMarkup.Balance(once);
        Assert.That(twice, Is.EqualTo(once));
    }

    [Test]
    public void RawDoubleClosingColorReproducesTheCrash()
    {
        // Sanity-check the harness: the exact content from the bug report must underflow when
        // rendered raw, otherwise the tests below would prove nothing.
        Assert.That(RenderWouldThrow("[/color][/color]"), Is.True);
    }

    [Test]
    [TestCase("[/color][/color]")]
    [TestCase("[/color][/color][/color]")]
    [TestCase("hello [/color][/color] world")]
    [TestCase("[color=red]hi[/color][/color]")]
    [TestCase("[/font][/font]")]
    [TestCase("[color=red]unterminated")]
    [TestCase("[color=red][font]bad nesting[/color][/font]")]
    [TestCase("[head=2][bold]Title[/head]body[/bold]")]
    [TestCase("[bold][color=red]X[/bold]Y")]
    [TestCase("[color=red][color=green]X[/color][/color][/color]")]
    [TestCase("")]
    [TestCase("plain document text, no tags")]
    [TestCase("[color=#ff0000]valid[/color]")]
    public void BalancedContentNeverUnderflows(string raw)
    {
        var balanced = CharacterDocumentMarkup.BuildBalancedMessage(raw);
        Assert.That(RenderWouldThrow(balanced), Is.False,
            $"Balanced markup still underflows the render stack: '{balanced.ToMarkup()}'");
    }

    [Test]
    public void BalancingIsIdempotent()
    {
        const string raw = "[color=red]hi[/color][/color] [font]x";
        var once = CharacterDocumentMarkup.Balance(raw);
        var twice = CharacterDocumentMarkup.Balance(once);
        Assert.That(twice, Is.EqualTo(once));
    }

    [Test]
    public void WellFormedFormattingTextIsPreserved()
    {
        // Balancing must not corrupt the visible text of already-valid content.
        var msg = CharacterDocumentMarkup.BuildBalancedMessage("[color=red]Red[/color] and [bold]bold[/bold]");
        Assert.That(msg.ToString(), Is.EqualTo("Red and bold"));
    }

    [Test]
    public void MultilineTextIsPreserved()
    {
        // Documents are multi-line; newlines must survive the parse/rebuild round-trip.
        var balanced = CharacterDocumentMarkup.Balance("line one\nline two\nline three");
        Assert.That(FormattedMessage.FromMarkupPermissive(balanced).ToString(),
            Is.EqualTo("line one\nline two\nline three"));
    }

    [Test]
    public void NullContentIsHandled()
    {
        Assert.That(CharacterDocumentMarkup.Balance(null!), Is.Empty);
    }
}
