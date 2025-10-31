using Content.Shared.Implants.Components;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed partial class TriggerOnActivateImplantSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed partial class TriggerOnActivateImplantSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnActivateImplantComponent, ActivateImplantEvent>(OnActivateImplant);
    }

    private void OnActivateImplant(Entity<TriggerOnActivateImplantComponent> ent, ref ActivateImplantEvent args)
    {
<<<<<<< HEAD
        _trigger.Trigger(ent.Owner, args.Performer, ent.Comp.KeyOut);
=======
        Trigger.Trigger(ent.Owner, args.Performer, ent.Comp.KeyOut);
>>>>>>> upstream/master
        args.Handled = true;
    }
}
