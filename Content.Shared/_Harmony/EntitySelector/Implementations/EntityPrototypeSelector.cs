// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 youtissoum <51883137+youtissoum@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared._Harmony.EntitySelector.Implementations;

public sealed partial class EntityPrototypeSelector : EntitySelector
{
    [DataField(required: true)]
    public EntProtoId Prototype;

    /// <inheritdoc />
    public override bool Matches(EntityUid entity)
    {
        if (!base.Matches(entity))
            return false;

        if (!EntityManager.TryGetComponent<MetaDataComponent>(entity, out var metaData))
            return false;

        if (metaData.EntityPrototype != null &&
            metaData.EntityPrototype.ID == Prototype)
            return true;

        return false;
    }
}
