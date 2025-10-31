using Content.Shared.Throwing;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed partial class TriggerOnLandSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed partial class TriggerOnLandSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnLandComponent, LandEvent>(OnLand);
    }

    private void OnLand(Entity<TriggerOnLandComponent> ent, ref LandEvent args)
    {
<<<<<<< HEAD
        _trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
=======
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
>>>>>>> upstream/master
    }
}
