using Content.Server._SV.GameTicking.Commands;
using NUnit.Framework;
using Robust.Shared.Maths;

namespace Content.Tests.Server._SV.GameTicking;

[TestFixture, TestOf(typeof(MapVoteSVCommand))]
[Parallelizable(ParallelScope.All)]
public sealed class MapVoteSVCommandTest
{
    [Test]
    public void Highlightcolor_IsParsableHexColor()
    {
        Assert.That(Color.TryFromHex(MapVoteSVCommand.Highlightcolor, out _), Is.True);
    }
}
