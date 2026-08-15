// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

#nullable enable
using Content.IntegrationTests.Fixtures;
using Content.IntegrationTests.Fixtures.Attributes;
using Content.Server._SV.CharacterDocuments.Consoles;
using Content.Shared.StationRecords;
using Content.Shared.StationRecords.Components;
using Content.Shared.StationRecords.Systems;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;

namespace Content.IntegrationTests.Tests._SV.CharacterDocuments;

/// <summary>
///     The security documents console shows a crew member's fingerprint, which it has to fetch from
///     that station's records. Documents are keyed by ProfileId but station records are keyed by an
///     opaque uint, so the only link between them is the character's name — that match is the part
///     that breaks, and it is what these tests cover.
/// </summary>
public sealed class CharacterDocumentConsoleFingerprintTest : GameTest
{
    private const string CrewName = "Alice Kane";
    private const string CrewFingerprint = "VXK92MC01LL8T";

    [SidedDependency(Side.Server)]
    private readonly CharacterDocumentConsoleSystem _docConsole = null!;

    [SidedDependency(Side.Server)]
    private readonly StationRecordsSystem _stationRecords = null!;

    public override PoolSettings PoolSettings => new()
    {
        Connected = false
    };

    /// <summary>
    ///     Builds a records holder carrying one general record for <see cref="CrewName"/>.
    /// </summary>
    private Entity<StationRecordsComponent> MakeStationWithRecord(string name, string? fingerprint)
    {
        var station = SEntMan.Spawn();
        var records = SEntMan.AddComponent<StationRecordsComponent>(station);

        _stationRecords.AddRecordEntry((station, records),
            new GeneralStationRecord { Name = name, Fingerprint = fingerprint });

        return (station, records);
    }

    [Test]
    public async Task FingerprintIsFoundByCharacterName()
    {
        await Server.WaitAssertion(() =>
        {
            var station = MakeStationWithRecord(CrewName, CrewFingerprint);

            Assert.That(_docConsole.TryGetRecordKeyByName(station, CrewName, out var key), Is.True,
                "The console could not match a crew member to their own station record.");
            Assert.That(_stationRecords.TryGetRecord<GeneralStationRecord>(key, out var record, station.Comp), Is.True);
            Assert.That(record!.Fingerprint, Is.EqualTo(CrewFingerprint));
        });
    }

    [Test]
    public async Task UnknownCharacterNameResolvesNothing()
    {
        await Server.WaitAssertion(() =>
        {
            var station = MakeStationWithRecord(CrewName, CrewFingerprint);

            Assert.That(_docConsole.TryGetRecordKeyByName(station, "Nobody Here", out _), Is.False,
                "A name with no station record resolved to a key anyway.");
        });
    }

    [Test]
    public async Task RecordWithoutFingerprintResolvesNull()
    {
        // Records made before a fingerprint was available keep a null, which the console renders as N/A.
        await Server.WaitAssertion(() =>
        {
            var station = MakeStationWithRecord(CrewName, null);

            Assert.That(_docConsole.TryGetRecordKeyByName(station, CrewName, out var key), Is.True);
            Assert.That(_stationRecords.TryGetRecord<GeneralStationRecord>(key, out var record, station.Comp), Is.True);
            Assert.That(record!.Fingerprint, Is.Null);
        });
    }
}
