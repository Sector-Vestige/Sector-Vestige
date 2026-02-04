// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 OnyxTheBrave <vinjeerik@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared._SV.EyeTracker;

/// <summary>
/// Get and network the current eye rotation
/// </summary>
[Serializable, NetSerializable]
public sealed class GetEyeRotationEvent(NetEntity ent, NetEntity player) : EntityEventArgs
{
    public NetEntity NetEntity { get; set; } = ent;
    public NetEntity PlayerEntity { get; set; } = player;
}

[Serializable, NetSerializable]
public sealed class GetNetworkedEyeRotationEvent(NetEntity ent, Angle angle) : EntityEventArgs
{
    public NetEntity NetEntity { get; set; } = ent;
    public Angle Angle { get; set; } = angle;
}
