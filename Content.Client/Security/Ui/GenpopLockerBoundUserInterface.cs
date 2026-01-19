// SPDX-FileCopyrightText: 2026 Wizards Den contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 OnyxTheBrave <131422822+OnyxTheBrave@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Security.Components;
using JetBrains.Annotations;

namespace Content.Client.Security.Ui;

[UsedImplicitly]
public sealed class GenpopLockerBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private GenpopLockerMenu? _menu;

    protected override void Open()
    {
        base.Open();

        _menu = new(Owner, EntMan);

        _menu.OnConfigurationComplete += (name, time, crime) =>
        {
            SendPredictedMessage(new GenpopLockerIdConfiguredMessage(name, time, crime));
            Close();
        };

        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _menu?.Orphan();
        _menu = null;
    }
}

