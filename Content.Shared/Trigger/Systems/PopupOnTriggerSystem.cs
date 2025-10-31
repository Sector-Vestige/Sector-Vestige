using Content.Shared.Popups;
using Content.Shared.Trigger.Components.Effects;

namespace Content.Shared.Trigger.Systems;

/// <summary>
/// This handles <see cref="PopupOnTriggerComponent"/>
/// </summary>
<<<<<<< HEAD
public sealed class PopupOnTriggerSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PopupOnTriggerComponent, TriggerEvent>(OnTrigger);
    }

    private void OnTrigger(Entity<PopupOnTriggerComponent> ent, ref TriggerEvent args)
    {
        if (args.Key != null && !ent.Comp.KeysIn.Contains(args.Key))
            return;

        var target = ent.Comp.TargetUser ? args.User : ent.Owner;

        if (target == null)
            return;

=======
public sealed class PopupOnTriggerSystem : XOnTriggerSystem<PopupOnTriggerComponent>
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    protected override void OnTrigger(Entity<PopupOnTriggerComponent> ent, EntityUid target, ref TriggerEvent args)
    {
>>>>>>> upstream/master
        // Popups only play for one entity
        if (ent.Comp.Quiet)
        {
            if (ent.Comp.Predicted)
<<<<<<< HEAD
                _popup.PopupClient(Loc.GetString(ent.Comp.Text),
                                    target.Value,
                                    ent.Comp.UserIsRecipient ? args.User : ent.Owner,
                                    ent.Comp.PopupType);

            else if (args.User != null)
                _popup.PopupEntity(Loc.GetString(ent.Comp.OtherText ?? ent.Comp.Text),
                                    target.Value,
                                    args.User.Value,
                                    ent.Comp.PopupType);
=======
            {
                _popup.PopupClient(Loc.GetString(ent.Comp.Text),
                    target,
                    ent.Comp.UserIsRecipient ? args.User : ent.Owner,
                    ent.Comp.PopupType);
            }

            else if (args.User != null)
            {
                _popup.PopupEntity(Loc.GetString(ent.Comp.OtherText ?? ent.Comp.Text),
                    target,
                    args.User.Value,
                    ent.Comp.PopupType);
            }
>>>>>>> upstream/master

            return;
        }

        // Popups play for all entities
        if (ent.Comp.Predicted)
<<<<<<< HEAD
            _popup.PopupPredicted(Loc.GetString(ent.Comp.Text),
                                Loc.GetString(ent.Comp.OtherText ?? ent.Comp.Text),
                                target.Value,
                                ent.Comp.UserIsRecipient ? args.User : ent.Owner,
                                ent.Comp.PopupType);

        else
            _popup.PopupEntity(Loc.GetString(ent.Comp.OtherText ?? ent.Comp.Text),
                                target.Value,
                                ent.Comp.PopupType);
=======
        {
            _popup.PopupPredicted(Loc.GetString(ent.Comp.Text),
                Loc.GetString(ent.Comp.OtherText ?? ent.Comp.Text),
                target,
                ent.Comp.UserIsRecipient ? args.User : ent.Owner,
                ent.Comp.PopupType);
        }

        else
        {
            _popup.PopupEntity(Loc.GetString(ent.Comp.OtherText ?? ent.Comp.Text),
                target,
                ent.Comp.PopupType);
        }
>>>>>>> upstream/master
    }
}
