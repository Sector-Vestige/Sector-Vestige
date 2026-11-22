using Content.Server.Body.Systems;
using Content.Shared.Body.Systems;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server._CD.Traits;

public sealed partial class SynthSystem : EntitySystem
{
    [Dependency] private BloodstreamSystem _bloodstream = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SynthComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<SynthComponent, BeforeShowTypingIndicatorEvent>(OnBeforeShowTypingIndicator);
    }

    private void OnStartup(EntityUid uid, SynthComponent component, ComponentStartup args)
    {
        // Ensure they have a typing indicator component
        EnsureComp<TypingIndicatorComponent>(uid);

        // Give them synth blood. Ion storm notif is handled in that system
        _bloodstream.ChangeBloodReagents(uid, component.BloodReferenceSolution);
    }

    private void OnBeforeShowTypingIndicator(EntityUid uid, SynthComponent component, BeforeShowTypingIndicatorEvent args)
    {
        // Override the typing indicator to use the robot indicator
        args.TryUpdateTimeAndIndicator(component.TypingIndicatorPrototype, null);
    }
}
