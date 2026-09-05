// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 J <billsmith116@gmail.com>
// SPDX-FileCopyrightText: 2025 ScarKy0 <106310278+ScarKy0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.EntityTable.EntitySelectors;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
namespace Content.Shared.Delivery;

/// <summary>
/// Used to mark entities that are valid for spawning deliveries on.
/// If this requires power, it needs to be powered to count as a valid spawner.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class DeliverySpawnerComponent : Component
{
    /// <summary>
    /// The entity table to select deliveries from.
    /// </summary>
    [DataField(required: true)]
    public EntityTableSelector Table = default!;

    /// <summary>
    /// The max amount of deliveries this spawner can hold at a time.
    /// </summary>
    [DataField]
    public int MaxContainedDeliveryAmount = 20;

    /// <summary>
    /// The currently held amount of deliveries.
    /// They are stored as an int and only spawned on use, as to not create additional entities without the need to.
    /// </summary>
    [DataField, AutoNetworkedField]
    public int ContainedDeliveryAmount;

    /// <summary>
    /// The sound to play when the spawner spawns a delivery.
    /// </summary>
    [DataField]
    public SoundSpecifier? SpawnSound = new SoundCollectionSpecifier("DeliverySpawnSounds", AudioParams.Default.AddVolume(-9)); // SV - Mail additions (-7 -> -9) - This is loud AF

    /// <summary>
    /// The sound to play when a spawner is opened, and spills all the deliveries out.
    /// </summary>
    [DataField]
    public SoundSpecifier? OpenSound = new SoundCollectionSpecifier("storageRustle");

    // SV mail additons - start

    /// <summary>
    /// The container ID in where the mail is stored
    /// </summary>
    [DataField]
    public string ContainerId = "maildeliveries";

    // SV mail additons - end
}
