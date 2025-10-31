<<<<<<< HEAD
using Content.Shared.Damage;
=======
>>>>>>> upstream/master
using Content.Shared.Damage.Systems;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

<<<<<<< HEAD
public sealed class StaminaDamageOnTriggerSystem : EntitySystem
{
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StaminaDamageOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<StaminaDamageOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

        var ev = new BeforeStaminaDamageOnTriggerEvent(ent.Comp.Stamina, target.Value);
        RaiseLocalEvent(ent.Owner, ref ev);

        _stamina.TakeStaminaDamage(target.Value, ev.Stamina, source: args.User, with: ent.Owner, ignoreResist: ent.Comp.IgnoreResistances);
=======
public sealed class StaminaDamageOnTriggerSystem : XOnTriggerSystem<StaminaDamageOnTriggerComponent>
{
    [Dependency] private readonly SharedStaminaSystem _stamina = default!;

    protected override void OnTrigger(Entity<StaminaDamageOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
        var ev = new BeforeStaminaDamageOnTriggerEvent(ent.Comp.Stamina, target);
        RaiseLocalEvent(ent.Owner, ref ev);

        _stamina.TakeStaminaDamage(target, ev.Stamina, source: args.User, with: ent.Owner, ignoreResist: ent.Comp.IgnoreResistances);
>>>>>>> upstream/master

        args.Handled = true;
    }
}

/// <summary>
/// Raised on an entity before it inflicts stamina due to StaminaDamageOnTriggerComponent.
/// Used to modify the stamina that will be inflicted.
/// </summary>
[ByRefEvent]
public record struct BeforeStaminaDamageOnTriggerEvent(float Stamina, EntityUid Tripper);
