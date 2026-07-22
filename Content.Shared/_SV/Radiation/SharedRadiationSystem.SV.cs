// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Radiation.Components;

// Sector Vestige: partial extension of the upstream SharedRadiationSystem.
// This lives in a fork-owned _SV/ file (merge-safe) but MUST declare the upstream
// namespace so it binds to the same partial class. Being part of SharedRadiationSystem
// is what satisfies [Access(typeof(SharedRadiationSystem))] on RadiationSourceComponent,
// which forbids writing its fields from any other system.
namespace Content.Shared.Radiation.Systems;

public abstract partial class SharedRadiationSystem
{
    /// <summary>
    ///     Sector Vestige - Sets the slope (radiation falloff over distance) of a
    ///     <see cref="RadiationSourceComponent"/>. Upstream only exposes SetIntensity;
    ///     the supermatter (_EE) needs a dynamic slope, so we add the matching setter here.
    /// </summary>
    /// <param name="entity">Radiation source we're attempting to update.</param>
    /// <param name="slope">Slope we're setting the source to.</param>
    public void SetSlope(Entity<RadiationSourceComponent?> entity, float slope)
    {
        if (!SourceQuery.Resolve(entity, ref entity.Comp, false))
            return;

        entity.Comp.Slope = slope;
    }
}
