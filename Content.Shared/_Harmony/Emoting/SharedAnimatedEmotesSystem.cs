// SPDX-FileCopyrightText: 2026 Harmony contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 jajsha <corbinbinouche7@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 FluffMe <1780586+FluffMe@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

// Original code by whateverusername0 from Goob-Station at commit 3022db4
// Available at: https://github.com/Goob-Station/Goob-Station/blob/3022db48e89ff00b762004767e7850023df3ee97/Content.Shared/_Goobstation/Emoting/SharedAnimatedEmotesSystem.cs

using Robust.Shared.GameStates;

namespace Content.Shared._Harmony.Emoting;

public abstract class SharedAnimatedEmotesSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AnimatedEmotesComponent, ComponentGetState>(OnGetState);
    }

    private void OnGetState(Entity<AnimatedEmotesComponent> ent, ref ComponentGetState args)
    {
        args.State = new AnimatedEmotesComponentState(ent.Comp.Emote);
    }
}
