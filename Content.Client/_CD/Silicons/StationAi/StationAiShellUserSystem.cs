// SPDX-FileCopyrightText: 2026 Cosmatic Drift contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared._CD.Silicons.StationAi;
using Robust.Client.GameObjects;

namespace Content.Client._CD.Silicons.StationAi;

public sealed class StationAiShellUserSystem : SharedStationAiShellUserSystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<StationAiShellUserComponent, AfterAutoHandleStateEvent>(OnShellUserState);
    }

    private void OnShellUserState(Entity<StationAiShellUserComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        if (UserInterface.TryGetOpenUi<AiShellSelectionBoundUserInterface>(ent.Owner, ShellUiKey.Key, out var bui))
        {
            bui.Refresh(ent);
        }
    }
}
