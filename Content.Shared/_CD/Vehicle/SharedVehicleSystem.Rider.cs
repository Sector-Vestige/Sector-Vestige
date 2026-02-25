// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2023 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Anzuneth <malachigene@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Hands;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared._CD.Vehicle.Components;
using Robust.Shared.GameStates;

namespace Content.Shared._CD.Vehicle;

public abstract partial class SharedVehicleSystem
{
    private void InitializeRider()
    {
        SubscribeLocalEvent<RiderComponent, ComponentGetState>(OnRiderGetState);
        SubscribeLocalEvent<RiderComponent, VirtualItemDeletedEvent>(OnVirtualItemDeleted);
        SubscribeLocalEvent<RiderComponent, PullAttemptEvent>(OnPullAttempt);
    }

    private void OnRiderGetState(EntityUid uid, RiderComponent component, ref ComponentGetState args)
    {
        args.State = new RiderComponentState()
        {
            Entity = GetNetEntity(component.Vehicle),
        };
    }

    /// <summary>
    /// Kick the rider off the vehicle if they press q / drop the virtual item
    /// </summary>
    private void OnVirtualItemDeleted(EntityUid uid, RiderComponent component, VirtualItemDeletedEvent args)
    {
        if (args.BlockingEntity == component.Vehicle)
        {
            _buckle.TryUnbuckle(uid, uid, true);
        }
    }

    private void OnPullAttempt(EntityUid uid, RiderComponent component, PullAttemptEvent args)
    {
        if (component.Vehicle != null)
            args.Cancelled = true;
    }
}
