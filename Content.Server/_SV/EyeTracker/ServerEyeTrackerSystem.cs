// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <131422822+OnyxTheBrave@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <vinjeerik@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._SV.EyeTracker;

namespace Content.Server._SV.EyeTracker;

[Access(typeof(EyeTrackerComponent))]
public sealed class ServerEyeTrackerSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    public override void Initialize()
    {
        SubscribeAllEvent<GetNetworkedEyeRotationEvent>(SetServerEyeRotation);
        base.Initialize();
    }

    private void SetServerEyeRotation(GetNetworkedEyeRotationEvent args)
    {
        if (!_entityManager.TryGetComponent<EyeTrackerComponent>(_entityManager.GetEntity(args.NetEntity), out var tracker))
            return;

        tracker.Rotation = args.Angle;
        Dirty(_entityManager.GetEntity(args.NetEntity), tracker);
    }
}
