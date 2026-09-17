// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

#nullable enable
using Content.IntegrationTests.Fixtures;
using Content.Server.Maps;
using Content.Shared.Maps;
using Robust.Shared.ContentPack;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests._SV.Mappreview;

public sealed class MapPreviewTest : GameTest
{
    private readonly ProtoId<GameMapPoolPrototype> _svMapPool = "SVmapPool";
    public override PoolSettings PoolSettings => PsDisconnected;

    /// <summary>
    /// Every map in the SV vote pool must declare a preview image, and that image must exist in Resources,
    /// otherwise the mapvotesv buttons show a blank icon.
    /// </summary>
    [Test]
    public async Task CheckIfAllMapPrototypesHaveMapPreviews()
    {
        var protoMan = Server.ResolveDependency<IPrototypeManager>();
        var resMan = Server.ResolveDependency<IResourceManager>();
        var allMaps = protoMan.EnumeratePrototypes<GameMapPrototype>();

        // A missing pool must fail loudly, not silently pass an empty test.
        Assert.That(protoMan.TryIndex(_svMapPool, out var pool), Is.True, $"Pool '{_svMapPool}' not found");
        if (pool == null)
            return;

        Assert.Multiple(() =>
        {
            foreach (var map in allMaps)
            {
                // Only maps that can actually be voted on need a preview.
                if (!pool.Maps.Contains(map.ID))
                    continue;

                Assert.That(map.Preview, Is.Not.Null, $"Map '{map.ID}' is in pool '{pool.ID}' but has no preview set");

                if (map.Preview is { } preview)
                    Assert.That(resMan.ContentFileExists(preview), Is.True, $"Map '{map.ID}' preview '{preview}' does not exist in Resources");
            }
        });
    }
}
