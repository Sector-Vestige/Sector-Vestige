// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 youtissoum <51883137+youtissoum@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using JetBrains.Annotations;

namespace Content.Shared._Harmony.EntitySelector;

/// <summary>
/// Provides an API for using an <see cref="EntitySelector"/>
/// </summary>
public sealed class EntitySelectorManager
{
    [PublicAPI]
    public static bool EntityMatchesAny(EntityUid entity, IEnumerable<EntitySelector> selectors)
    {
        foreach (var selector in selectors)
        {
            if (selector.Matches(entity))
                return true;
        }

        return false;
    }

    [PublicAPI]
    public static IEnumerable<EntityUid> AllMatchingEntities(IEnumerable<EntityUid> entities, EntitySelector selector)
    {
        foreach (var entity in entities)
        {
            if (selector.Matches(entity))
                yield return entity;
        }
    }

    [PublicAPI]
    public static IEnumerable<EntityUid> AllEntitiesMatchingAny(
        IEnumerable<EntityUid> entities,
        List<EntitySelector> selectors)
    {
        foreach (var entity in entities)
        {
            foreach (var selector in selectors)
            {
                if (!selector.Matches(entity))
                    continue;

                yield return entity;
                break;
            }
        }
    }
}
