// SPDX-FileCopyrightText: 2026 Delta-V contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared._DV.Harpy
{
    [RegisterComponent, NetworkedComponent]
    public sealed partial class HarpySingerComponent : Component
    {
        [DataField("midiActionId", serverOnly: true,
            customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
        public string? MidiActionId = "ActionHarpyPlayMidi";

        [DataField("midiAction", serverOnly: true)] // server only, as it uses a server-BUI event !type
        public EntityUid? MidiAction;
    }
}
