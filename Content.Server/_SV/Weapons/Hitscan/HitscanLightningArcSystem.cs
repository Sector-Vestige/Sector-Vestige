using Content.Server.Lightning;
using Content.Shared._SV.Weapons.Hitscan;
using Content.Shared.Weapons.Hitscan.Events;

namespace Content.Server._SV.Weapons.Hitscan;

/// <summary>
/// Handles <see cref="HitscanLightningArcComponent"/>. When the hitscan raycast strikes a target,
/// fires a lightning beam at it and chains to nearby targets. Mirrors HitscanBasicDamageSystem,
/// which listens for the same <see cref="HitscanRaycastFiredEvent"/>.
/// </summary>
public sealed partial class HitscanLightningArcSystem : EntitySystem
{
    [Dependency] private LightningSystem _lightning = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HitscanLightningArcComponent, HitscanRaycastFiredEvent>(OnHitscanHit);
    }

    private void OnHitscanHit(Entity<HitscanLightningArcComponent> ent, ref HitscanRaycastFiredEvent args)
    {
        if (args.Data.HitEntity is not { } target)
            return;

        if (ent.Comp.ShootPrimaryBeam)
        {
            var shooter = args.Data.Shooter ?? args.Data.Gun;
            _lightning.ShootLightning(shooter, target, ent.Comp.LightningPrototype);
        }

        _lightning.ShootRandomLightnings(target, ent.Comp.Range, ent.Comp.BoltCount, ent.Comp.LightningPrototype, ent.Comp.ArcDepth);
    }
}
