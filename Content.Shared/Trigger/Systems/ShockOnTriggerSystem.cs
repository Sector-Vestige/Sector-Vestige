using Content.Shared.Electrocution;
using Content.Shared.Trigger.Components.Effects;
using Robust.Shared.Containers;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class ShockOnTriggerSystem : EntitySystem
=======
public sealed class ShockOnTriggerSystem : XOnTriggerSystem<ShockOnTriggerComponent>
>>>>>>> upstream/master
{
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedElectrocutionSystem _electrocution = default!;

<<<<<<< HEAD
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ShockOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<ShockOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        EntityUid? target;
=======
    protected override void OnTrigger(Entity<ShockOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        // Override the normal target if we target the container
>>>>>>> upstream/master
        if (ent.Comp.TargetContainer)
        {
            // shock whoever is wearing this clothing item
            if (!_container.TryGetContainingContainer(ent.Owner, out var container))
                return;
<<<<<<< HEAD
            target = container.Owner;
        }
        else
        {
            target = ent.Comp.TargetUser ? args.User : ent.Owner;
        }

        if (target == null)
            return;

        _electrocution.TryDoElectrocution(target.Value, null, ent.Comp.Damage, ent.Comp.Duration, true, ignoreInsulation: true);
        args.Handled = true;
    }

=======

            target = container.Owner;
        }

        _electrocution.TryDoElectrocution(target, null, ent.Comp.Damage, ent.Comp.Duration, true, ignoreInsulation: true);
        args.Handled = true;
    }
>>>>>>> upstream/master
}
