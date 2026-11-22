using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Robust.Shared.Timing;

namespace Content.Shared._SV.Effects;

/// <summary>
/// This will heal nearby entities a set of damage
/// </summary>
public sealed partial class HealNearbySystem : EntitySystem
{
    [Dependency] private IGameTiming _timing = default!;
    [Dependency] private EntityLookupSystem _lookup = default!;
    [Dependency] private DamageableSystem _damageableSystem = default!;

    private readonly HashSet<EntityUid> _entities = new();


    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HealNearbyComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(EntityUid uid, HealNearbyComponent component, MapInitEvent args)
    {
        component.NextHealTime = _timing.CurTime + component.HealTime;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<HealNearbyComponent>();
        while (query.MoveNext(out var uid, out var healNearbyComponent))
        {
            if (_timing.CurTime < healNearbyComponent.NextHealTime)
                continue;

            healNearbyComponent.NextHealTime += healNearbyComponent.HealTime;

            Dirty(uid, healNearbyComponent);

            _entities.Clear();
            _lookup.GetEntitiesInRange(uid, healNearbyComponent.Radius, _entities);

            if (_entities.Capacity == 0)
                continue;

            var damage = healNearbyComponent.Damage;

            foreach (var entity in _entities)
            {
                if (!TryComp<DamageableComponent>(entity,out _))
                    continue;

                _damageableSystem.ChangeDamage(entity, damage);
            }

        }
    }
}
