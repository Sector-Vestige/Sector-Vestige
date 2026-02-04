// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Examine;
using Robust.Shared.Utility;

namespace Content.Shared._CD.Engraving;

public abstract class SharedEngraveableSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EngraveableComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<EngraveableComponent> ent, ref ExaminedEvent args)
    {
        var msg = new FormattedMessage();
        msg.AddMarkupOrThrow(Loc.GetString(ent.Comp.EngravedMessage == string.Empty
            ? ent.Comp.NoEngravingText
            : ent.Comp.HasEngravingText));

        if (ent.Comp.EngravedMessage != string.Empty)
            msg.AddMarkupPermissive(Loc.GetString(ent.Comp.EngravedMessage));

        args.PushMessage(msg, 1);
    }
}
