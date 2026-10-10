// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 ElectroJr <leonsfriedrich@gmail.com>
// SPDX-FileCopyrightText: 2025 Kyle Tyo <36606155+VerinSenpai@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 B_Kirill <153602297+B-Kirill@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 OnyxTheBrave <131422822+OnyxTheBrave@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2026 Whatstone <166147148+whatston3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Robust.Shared.ContentPack;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map.Events;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Utility;

namespace Content.Server.Maps;

/// <summary>
///     Performs basic map migration operations by listening for engine <see cref="MapLoaderSystem"/> events.
///     SV - This whole file has been edited to suit our needs.
/// </summary>
public sealed partial class MapMigrationSystem : EntitySystem
{
    [Dependency] private IResourceManager _resMan = default!;

    /// <summary>
    ///     SV - The upstream migration file. Always read first so fork files can override it.
    /// </summary>
    private static readonly ResPath UpstreamMigrationFile = new("/migration.yml");

    /// <summary>
    ///     SV - Every .yml file under this directory is read as a migration file, sorted by path.
    ///     A later file overrides an earlier one for the same prototype id.
    /// </summary>
    private static readonly ResPath ForkMigrationDirectory = new("/_SV/migrations/");

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BeforeEntityReadEvent>(OnBeforeReadEvent);

#if DEBUG
        var mappings = ReadMigrations();

        // Verify that all of the entries map to valid entity prototypes.
        foreach (var newId in mappings.Values) // SV - Foreach loop though the
        {
            if (!string.IsNullOrEmpty(newId) && newId != "null")
                DebugTools.Assert(ProtoMan.HasIndex<EntityPrototype>(newId), $"{newId} is not an entity prototype.");
        }
#endif
    }

    /// <summary>
    /// SV - Rewritten to handle a handle a list of ResPath's instead of a singular one.
    /// </summary>
    private Dictionary<string, string> ReadMigrations()
    {
        var mappings = new Dictionary<string, string>();

        foreach (var path in GetMigrationFiles())
        {
            if (!_resMan.TryContentFileRead(path, out var stream))
                continue;

            using var reader = new StreamReader(stream, EncodingHelpers.UTF8);
            var document = DataNodeParser.ParseYamlStream(reader).FirstOrDefault();

            if (document?.Root is not MappingDataNode fileMappings)
                continue;

            foreach (var (key, value) in fileMappings)
            {
                if (value is not ValueDataNode valueNode)
                    continue;

                if (mappings.ContainsKey(key))
                    Log.Warning($"Migration for {key} in {path} overrides an earlier migration file.");

                mappings[key] = valueNode.Value;
            }
        }

        return mappings;
    }

    /// <summary>
    /// SV - The upstream file followed by every fork migration file, in a stable order.
    /// </summary>
    private IEnumerable<ResPath> GetMigrationFiles()
    {
        yield return UpstreamMigrationFile;

        // ContentFindFiles is recursive and makes no ordering guarantee, so sort for deterministic overrides.
        var forkFiles = _resMan.ContentFindFiles(ForkMigrationDirectory)
            .Where(path => path.Extension == "yml")
            .OrderBy(path => path.ToString(), StringComparer.Ordinal);

        foreach (var path in forkFiles)
            yield return path;
    }

    /// <summary>
    /// SV - Rewritten to handle the list from ReadMigrations
    /// </summary>
    private void OnBeforeReadEvent(BeforeEntityReadEvent ev)
    {
        foreach (var (key, value) in ReadMigrations())
        {
            if (string.IsNullOrWhiteSpace(value) || value == "null")
                ev.DeletedPrototypes.Add(key);
            else
                ev.RenamedPrototypes.Add(key, value);
        }
    }
}
