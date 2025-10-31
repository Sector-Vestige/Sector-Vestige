using Content.Shared.Trigger.Components.Triggers;
using Content.Shared.Weapons.Ranged.Events;

namespace Content.Shared.Trigger.Systems;
<<<<<<< HEAD
public sealed partial class TriggerOnEmptyGunshotSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed partial class TriggerOnEmptyGunshotSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnEmptyGunshotComponent, OnEmptyGunShotEvent>(OnEmptyGunShot);
    }

    private void OnEmptyGunShot(Entity<TriggerOnEmptyGunshotComponent> ent, ref OnEmptyGunShotEvent args)
    {
<<<<<<< HEAD
        _trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
=======
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
>>>>>>> upstream/master
    }
}
