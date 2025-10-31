<<<<<<< HEAD
using Content.Shared.Trigger;
=======
>>>>>>> upstream/master
using Content.Shared.Trigger.Components.Effects;
using Content.Shared.RepulseAttract;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class RepulseAttractOnTriggerSystem : EntitySystem
=======
public sealed class RepulseAttractOnTriggerSystem : XOnTriggerSystem<RepulseAttractOnTriggerComponent>
>>>>>>> upstream/master
{
    [Dependency] private readonly RepulseAttractSystem _repulse = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

<<<<<<< HEAD
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RepulseAttractOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<RepulseAttractOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        var position = _transform.GetMapCoordinates(target.Value);
=======
    protected override void OnTrigger(Entity<RepulseAttractOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        var position = _transform.GetMapCoordinates(target);
>>>>>>> upstream/master
        _repulse.TryRepulseAttract(position, args.User, ent.Comp.Speed, ent.Comp.Range, ent.Comp.Whitelist, ent.Comp.CollisionMask);

        args.Handled = true;
    }
}
