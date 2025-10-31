using Content.Shared.GameTicking;
using Content.Shared.Trigger.Components.Triggers;

namespace Content.Shared.Trigger.Systems;

/// <summary>
/// System for creating a trigger when the round ends.
/// </summary>
<<<<<<< HEAD
public sealed class TriggerOnRoundEndSystem : EntitySystem
{
    [Dependency] private readonly TriggerSystem _trigger = default!;

=======
public sealed class TriggerOnRoundEndSystem : TriggerOnXSystem
{
>>>>>>> upstream/master
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundEndMessageEvent>(OnRoundEnd);
    }

    private void OnRoundEnd(RoundEndMessageEvent args)
    {
        var triggerQuery = EntityQueryEnumerator<TriggerOnRoundEndComponent>();

        // trigger everything with the component
        while (triggerQuery.MoveNext(out var uid, out var comp))
        {
<<<<<<< HEAD
            _trigger.Trigger(uid, null, comp.KeyOut);
=======
            Trigger.Trigger(uid, null, comp.KeyOut);
>>>>>>> upstream/master
        }
    }
}
