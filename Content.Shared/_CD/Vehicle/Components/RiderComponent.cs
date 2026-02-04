// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Anzuneth <malachigene@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._CD.Vehicle.Components;

/// <summary>
/// Added to people when they are riding in a vehicle
/// used mostly to keep track of them for entityquery.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class RiderComponent : Component
{
    /// <summary>
    /// The vehicle this rider is currently riding.
    /// </summary>
    [ViewVariables] public EntityUid? Vehicle;

    public override bool SendOnlyToOwner => true;
}

[Serializable, NetSerializable]
public sealed class RiderComponentState : ComponentState
{
    public NetEntity? Entity;
}
