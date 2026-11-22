using Content.Shared.Chat;
using Content.Server.Popups;
using Content.Server.Speech.EntitySystems;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Puppet;
using Content.Shared.Speech;
using Content.Shared.Speech.Muting;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffectNew;
using Content.Shared._Harmony.Speech.Hypophonia;

namespace Content.Server._Harmony.Speech.Hypophonia
{
    public sealed partial class HypophoniaSystem : EntitySystem
    {
        [Dependency] private PopupSystem _popupSystem = default!;
        [Dependency] private StatusEffectsSystem _statusEffects = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<HypophoniaComponent, SpeakAttemptEvent>(OnSpeakAttempt);
            SubscribeLocalEvent<HypophoniaComponent, EmoteEvent>(OnEmote, before: new[] { typeof(VocalSystem) });
        }

        private void OnEmote(EntityUid uid, HypophoniaComponent component, ref EmoteEvent args)
        {
            if (args.Handled)
                return;

            // Let MutingSystem handle the event for muted characters (mimes included)
            if (_statusEffects.HasEffectComp<MutedStatusEffectComponent>(uid))
                return;

            //still leaves the text so it looks like they are pantomiming a laugh
            if (args.Emote.Category.HasFlag(EmoteCategory.Vocal))
                args.Handled = true;
        }

        private void OnSpeakAttempt(EntityUid uid, HypophoniaComponent component, SpeakAttemptEvent args)
        {
            // Let MutingSystem handle the event for puppets and muted characters (mimes included)
            if (HasComp<VentriloquistPuppetComponent>(uid) || _statusEffects.HasEffectComp<MutedStatusEffectComponent>(uid))
                return;

            // Allow whispering - Hypophonia means you can only whisper
            if (args.IsWhisper)
                return;

            // Cancel the event and show the popup for normal speech
            _popupSystem.PopupEntity(Loc.GetString("speech-hypophonia"), uid, uid);
            args.Cancel();
        }
    }
}
