using Content.Shared.Tools.Components;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class TriggerOnToolUseSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed class TriggerOnToolUseSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TriggerOnSimpleToolUsageComponent, SimpleToolDoAfterEvent>(OnToolUse);
    }

    private void OnToolUse(Entity<TriggerOnSimpleToolUsageComponent> ent, ref SimpleToolDoAfterEvent args)
    {
<<<<<<< HEAD
        _trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
=======
        Trigger.Trigger(ent.Owner, args.User, ent.Comp.KeyOut);
>>>>>>> upstream/master
    }
}
