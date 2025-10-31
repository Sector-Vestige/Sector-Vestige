using Content.Shared.Flash;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class FlashOnTriggerSystem : EntitySystem
{
    [Dependency] private readonly SharedFlashSystem _flash = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FlashOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<FlashOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        _flash.FlashArea(target.Value, args.User, ent.Comp.Range, ent.Comp.Duration, probability: ent.Comp.Probability);
=======
public sealed class FlashOnTriggerSystem : XOnTriggerSystem<FlashOnTriggerComponent>
{
    [Dependency] private readonly SharedFlashSystem _flash = default!;

    protected override void OnTrigger(Entity<FlashOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        _flash.FlashArea(target, args.User, ent.Comp.Range, ent.Comp.Duration, probability: ent.Comp.Probability);
>>>>>>> upstream/master
        args.Handled = true;
    }
}
