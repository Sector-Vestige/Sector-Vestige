using Content.Shared.Flash.Components;
<<<<<<< HEAD
using Content.Shared.Damage;
=======
using Content.Shared.Damage.Systems;
>>>>>>> upstream/master

namespace Content.Shared.Flash;

public sealed class DamagedByFlashingSystem : EntitySystem
{
    [Dependency] private readonly DamageableSystem _damageable = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<DamagedByFlashingComponent, FlashAttemptEvent>(OnFlashAttempt);
    }

    // TODO: Attempt events should not be doing state changes. But using AfterFlashedEvent does not work because this entity cannot get the status effect.
    // Best wait for Ed's status effect system rewrite.
    private void OnFlashAttempt(Entity<DamagedByFlashingComponent> ent, ref FlashAttemptEvent args)
    {
<<<<<<< HEAD
        _damageable.TryChangeDamage(ent, ent.Comp.FlashDamage);
=======
        _damageable.ChangeDamage(ent.Owner, ent.Comp.FlashDamage);
>>>>>>> upstream/master

        // TODO: It would be more logical if different flashes had different power,
        // and the damage would be inflicted depending on the strength of the flash.
    }
}
