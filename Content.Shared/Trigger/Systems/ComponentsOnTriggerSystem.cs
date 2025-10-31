using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed partial class ComponentsOnTriggerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AddComponentsOnTriggerComponent, TriggerEvent>(HandleAddTrigger);
        SubscribeLocalEvent<RemoveComponentsOnTriggerComponent, TriggerEvent>(HandleRemoveTrigger);
        SubscribeLocalEvent<ToggleComponentsOnTriggerComponent, TriggerEvent>(HandleToggleTrigger);
    }

    private void HandleAddTrigger(Entity<AddComponentsOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        if (ent.Comp.TriggerOnce && ent.Comp.Triggered)
            return;

        EntityManager.AddComponents(target.Value, ent.Comp.Components, ent.Comp.RemoveExisting);
=======
public sealed partial class AddComponentsOnTriggerSystem : XOnTriggerSystem<AddComponentsOnTriggerComponent>
{
    protected override void OnTrigger(Entity<AddComponentsOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (ent.Comp.TriggerOnce && ent.Comp.Triggered)
            return;

        EntityManager.AddComponents(target, ent.Comp.Components, ent.Comp.RemoveExisting);
>>>>>>> upstream/master
        ent.Comp.Triggered = true;
        Dirty(ent);

        args.Handled = true;
    }
<<<<<<< HEAD

    private void HandleRemoveTrigger(Entity<RemoveComponentsOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        if (ent.Comp.TriggerOnce && ent.Comp.Triggered)
            return;

        EntityManager.RemoveComponents(target.Value, ent.Comp.Components);
=======
}

public sealed partial class RemoveComponentsOnTriggerSystem : XOnTriggerSystem<RemoveComponentsOnTriggerComponent>
{
    protected override void OnTrigger(Entity<RemoveComponentsOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (ent.Comp.TriggerOnce && ent.Comp.Triggered)
            return;

        EntityManager.RemoveComponents(target, ent.Comp.Components);
>>>>>>> upstream/master
        ent.Comp.Triggered = true;
        Dirty(ent);

        args.Handled = true;
    }
<<<<<<< HEAD

    private void HandleToggleTrigger(Entity<ToggleComponentsOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        if (!ent.Comp.ComponentsAdded)
            EntityManager.AddComponents(target.Value, ent.Comp.Components, ent.Comp.RemoveExisting);
        else
            EntityManager.RemoveComponents(target.Value, ent.Comp.Components);
=======
}

public sealed partial class ToggleComponentsOnTriggerSystem : XOnTriggerSystem<ToggleComponentsOnTriggerComponent>
{
    protected override void OnTrigger(Entity<ToggleComponentsOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (!ent.Comp.ComponentsAdded)
            EntityManager.AddComponents(target, ent.Comp.Components, ent.Comp.RemoveExisting);
        else
            EntityManager.RemoveComponents(target, ent.Comp.Components);
>>>>>>> upstream/master

        ent.Comp.ComponentsAdded = !ent.Comp.ComponentsAdded;
        Dirty(ent);

        args.Handled = true;
    }
}
