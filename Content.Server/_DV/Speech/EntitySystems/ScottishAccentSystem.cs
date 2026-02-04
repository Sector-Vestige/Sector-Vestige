// SPDX-FileCopyrightText: 2026 Delta-V contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._DV.Speech.Components;
using Content.Shared.Speech;
using Content.Server.Speech.EntitySystems;

namespace Content.Server._DV.Speech.EntitySystems;

public sealed class ScottishAccentSystem : EntitySystem
{
    [Dependency] private readonly ReplacementAccentSystem _replacement = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ScottishAccentComponent, AccentGetEvent>(OnAccentGet);
    }

    // converts left word when typed into the right word. For example typing you becomes ye.
    public string Accentuate(string message, ScottishAccentComponent component)
    {
        var msg = message;

        msg = _replacement.ApplyReplacements(msg, "scottish");

        return msg;
    }

    private void OnAccentGet(EntityUid uid, ScottishAccentComponent component, AccentGetEvent args)
    {
        args.Message = Accentuate(args.Message, component);
    }
}
