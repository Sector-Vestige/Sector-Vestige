using Content.Shared.Sticky;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class TriggerOnStuckSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed class TriggerOnStuckSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnStuckComponent, EntityStuckEvent>(OnStuck);
    }

    private void OnStuck(Entity<TriggerOnStuckComponent> ent, ref EntityStuckEvent args)
    {
<<<<<<< HEAD
        _trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
=======
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
>>>>>>> upstream/master
    }
}
