// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Station.Systems;
using Content.Shared._SV.Delivery;
using Content.Shared.Delivery;
using Content.Shared.Examine;
using Content.Shared.FingerprintReader;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Power.EntitySystems;
using Content.Shared.StationRecords;
using Content.Shared.StationRecords.Components;
using Content.Shared.StationRecords.Systems;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.Containers;

namespace Content.Server._SV.Delivery;

/// <summary>
/// Server side of player-addressable mail (letters and packages).
/// Populates the address UI with the owning station's crew listing, stores the chosen recipient,
/// and on send spawns a real delivery addressed to them — carrying whatever items were loaded —
/// into the station's mail teleporter.
/// </summary>
public sealed partial class AddressableDeliverySystem : EntitySystem
{
    [Dependency] private UserInterfaceSystem _ui = default!;
    [Dependency] private StationSystem _station = default!;
    [Dependency] private StationRecordsSystem _records = default!;
    [Dependency] private SharedAppearanceSystem _appearance = default!;
    [Dependency] private LabelSystem _label = default!;
    [Dependency] private FingerprintReaderSystem _fingerprintReader = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedPowerReceiverSystem _power = default!;
    [Dependency] private SharedContainerSystem _container = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AddressableDeliveryComponent, ExaminedEvent>(OnExamine);

        Subs.BuiEvents<AddressableDeliveryComponent>(DeliveryAddressUiKey.Key, subs =>
        {
            subs.Event<BoundUIOpenedEvent>(OnUiOpened);
            subs.Event<AddressableDeliverySelectMessage>(OnRecipientSelected);
            subs.Event<AddressableDeliverySendMessage>(OnSend);
        });
    }

    private void OnExamine(Entity<AddressableDeliveryComponent> ent, ref ExaminedEvent args)
    {
        if (ent.Comp.RecipientName is { } name)
        {
            args.PushMarkup(Loc.GetString("addressable-delivery-examine-addressed",
                ("recipient", name),
                ("job", ent.Comp.RecipientJobTitle ?? Loc.GetString("addressable-delivery-job-unknown"))));
        }
        else
        {
            args.PushMarkup(Loc.GetString("addressable-delivery-examine-unaddressed"));
        }
    }

    private void OnUiOpened(Entity<AddressableDeliveryComponent> ent, ref BoundUIOpenedEvent args)
    {
        UpdateUi(ent);
    }

    private void OnRecipientSelected(Entity<AddressableDeliveryComponent> ent, ref AddressableDeliverySelectMessage args)
    {
        var station = _station.GetOwningStation(ent.Owner);
        if (!TryComp<StationRecordsComponent>(station, out var records))
            return;

        var key = new StationRecordKey(args.RecordKey, station.Value);
        if (!_records.TryGetRecord<GeneralStationRecord>(key, out var record, records))
            return;

        ent.Comp.RecipientRecordKey = args.RecordKey;
        ent.Comp.RecipientName = record.Name;
        ent.Comp.RecipientJobTitle = record.JobTitle;
        ent.Comp.RecipientStation = station.Value;

        UpdateUi(ent);
    }

    private void OnSend(Entity<AddressableDeliveryComponent> ent, ref AddressableDeliverySendMessage args)
    {
        // Re-resolve the chosen record at send time so the spawned delivery matches the current crew data.
        // ALSO this if statement is hella long lol it loooong
        if (ent.Comp.RecipientStation is not { } station
            || ent.Comp.RecipientRecordKey is not { } recordKey
            || !TryComp<StationRecordsComponent>(station, out var records)
            || !_records.TryGetRecord<GeneralStationRecord>(new StationRecordKey(recordKey, station), out var record, records))
        {
            _popup.PopupEntity(Loc.GetString("addressable-delivery-send-no-recipient"), args.Actor, args.Actor);
            return;
        }

        if (FindSpawner(station) is not { } spawner)
        {
            _popup.PopupEntity(Loc.GetString("addressable-delivery-send-no-spawner"), args.Actor, args.Actor);
            return;
        }

        var delivery = SpawnInContainerOrDrop(ent.Comp.DeliveryProto, spawner.Owner, spawner.Comp.ContainerId);
        ApplyRecipient(delivery, record, station);
        TransferContents(ent.Owner, delivery);

        _popup.PopupEntity(Loc.GetString("addressable-delivery-send-success", ("recipient", record.Name)), args.Actor, args.Actor);

        QueueDel(ent.Owner);
    }

    private void UpdateUi(Entity<AddressableDeliveryComponent> ent)
    {
        var crew = new Dictionary<uint, string>();

        var station = _station.GetOwningStation(ent.Owner);
        if (HasComp<StationRecordsComponent>(station))
        {
            foreach (var (id, record) in _records.GetRecordsOfType<GeneralStationRecord>(station.Value))
            {
                crew[id] = Loc.GetString("addressable-delivery-crew-entry",
                    ("name", record.Name),
                    ("job", record.JobTitle));
            }
        }

        _ui.SetUiState(ent.Owner, DeliveryAddressUiKey.Key, new AddressableDeliveryBuiState(crew, ent.Comp.RecipientRecordKey));
    }

    /// <summary>
    /// Applies a chosen station record to a freshly spawned delivery: recipient details, label,
    /// job icon, and a fingerprint lock so only the recipient can open it.
    /// </summary>
    private void ApplyRecipient(EntityUid delivery, GeneralStationRecord record, EntityUid station)
    {
        if (!TryComp<DeliveryComponent>(delivery, out var comp))
            return;

        comp.RecipientName = record.Name;
        comp.RecipientJobTitle = record.JobTitle;
        comp.RecipientStation = station;

        _appearance.SetData(delivery, DeliveryVisuals.JobIcon, record.JobIcon);
        _label.Label(delivery, record.Name);

        if (TryComp<FingerprintReaderComponent>(delivery, out var reader) && record.Fingerprint != null)
            _fingerprintReader.AddAllowedFingerprint((delivery, reader), record.Fingerprint);

        Dirty(delivery, comp);
    }

    /// <summary>
    /// Moves everything loaded into the addressable item's storage into the spawned delivery's
    /// contents container, so the recipient receives it on opening.
    /// </summary>
    private void TransferContents(EntityUid source, EntityUid delivery)
    {
        if (!TryComp<DeliveryComponent>(delivery, out var deliveryComp))
            return;

        if (!_container.TryGetContainer(source, StorageComponent.ContainerId, out var storage))
            return;

        if (!_container.TryGetContainer(delivery, deliveryComp.Container, out var target))
            return;

        foreach (var item in storage.ContainedEntities.ToArray())
        {
            _container.Insert(item, target);
        }
    }

    /// <summary>
    /// Finds a powered mail teleporter belonging to the given station to route the delivery through.
    /// </summary>
    private Entity<DeliverySpawnerComponent>? FindSpawner(EntityUid station)
    {
        var query = EntityQueryEnumerator<DeliverySpawnerComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_station.GetOwningStation(uid) != station)
                continue;

            if (!_power.IsPowered(uid))
                continue;

            return (uid, comp);
        }

        return null;
    }
}
