using Content.Shared.Body.Systems;
using Content.Shared.Inventory;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class GibOnTriggerSystem : EntitySystem
=======
public sealed class GibOnTriggerSystem : XOnTriggerSystem<GibOnTriggerComponent>
>>>>>>> upstream/master
{
    [Dependency] private readonly SharedBodySystem _body = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;

<<<<<<< HEAD
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GibOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<GibOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        if (ent.Comp.DeleteItems)
        {
            var items = _inventory.GetHandOrInventoryEntities(target.Value);
=======
    protected override void OnTrigger(Entity<GibOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        if (ent.Comp.DeleteItems)
        {
            var items = _inventory.GetHandOrInventoryEntities(target);
>>>>>>> upstream/master
            foreach (var item in items)
            {
                PredictedQueueDel(item);
            }
        }
<<<<<<< HEAD
        _body.GibBody(target.Value, true);
=======

        _body.GibBody(target, true);
>>>>>>> upstream/master
        args.Handled = true;
    }
}
