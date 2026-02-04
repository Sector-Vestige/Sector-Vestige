// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 FluffMe <1780586+FluffMe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Chat;
using Content.Server.Popups;
using Content.Server.Speech.EntitySystems;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Puppet;
using Content.Shared.Speech;
using Content.Shared.Speech.Muting;
using Content.Shared._Harmony.Speech.Hypophonia;

namespace Content.Server._Harmony.Speech.Hypophonia
{
    public sealed class HypophoniaSystem : EntitySystem
    {
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<HypophoniaComponent, SpeakAttemptEvent>(OnSpeakAttempt);
            SubscribeLocalEvent<HypophoniaComponent, EmoteEvent>(OnEmote, before: new[] { typeof(VocalSystem) });
            SubscribeLocalEvent<HypophoniaComponent, ScreamActionEvent>(OnScreamAction, before: new[] { typeof(VocalSystem) });
        }

        private void OnEmote(EntityUid uid, HypophoniaComponent component, ref EmoteEvent args)
        {
            if (args.Handled)
                return;

            // Let MutingSystem handle the event for muted characters (mimes included)
            if (HasComp<MutedComponent>(uid))
                return;

            //still leaves the text so it looks like they are pantomiming a laugh
            if (args.Emote.Category.HasFlag(EmoteCategory.Vocal))
                args.Handled = true;
        }

        private void OnScreamAction(EntityUid uid, HypophoniaComponent component, ScreamActionEvent args)
        {
            if (args.Handled)
                return;

            // Let MutingSystem handle the event muted characters (mimes included)
            if (HasComp<MutedComponent>(uid))
                return;

            // Mark the event as handled and show the popup
            _popupSystem.PopupEntity(Loc.GetString("speech-hypophonia"), uid, uid);
            args.Handled = true;
        }


        private void OnSpeakAttempt(EntityUid uid, HypophoniaComponent component, SpeakAttemptEvent args)
        {
            // Let MutingSystem handle the event for puppets and muted characters (mimes included)
            if (HasComp<VentriloquistPuppetComponent>(uid) || HasComp<MutedComponent>(uid))
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
