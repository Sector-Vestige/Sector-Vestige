using Content.Shared.Slippery;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed partial class TriggerOnSlipSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed partial class TriggerOnSlipSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnSlipComponent, SlipEvent>(OnSlip);
    }

    private void OnSlip(Entity<TriggerOnSlipComponent> ent, ref SlipEvent args)
    {
<<<<<<< HEAD
        _trigger.Trigger(ent.Owner, args.Slipped, ent.Comp.KeyOut);
=======
        Trigger.Trigger(ent.Owner, args.Slipped, ent.Comp.KeyOut);
>>>>>>> upstream/master
    }
}
