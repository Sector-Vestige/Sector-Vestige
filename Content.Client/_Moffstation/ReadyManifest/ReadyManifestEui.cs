// SPDX-FileCopyrightText: 2026 Moffstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
// SPDX-FileCopyrightText: 2025 vestige-bot <vestige-bot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Client.Eui;
using Content.Shared._Moffstation.ReadyManifest;
using Content.Shared.Eui;
using JetBrains.Annotations;

namespace Content.Client._Moffstation.ReadyManifest;

[UsedImplicitly]
public sealed class ReadyManifestEui : BaseEui
{
    private readonly ReadyManifestUi _window;

    public ReadyManifestEui()
    {
        _window = new();

        _window.OnClose += () => SendMessage(new CloseEuiMessage());
    }

    public override void Opened()
    {
        _window.OpenCentered();
    }

    public override void Closed()
    {
        _window.Close();
    }

    public override void HandleState(EuiStateBase state)
    {
        if (state is not ReadyManifestEuiState cast)
            return;

        _window.RebuildUI(cast.JobCounts);
    }
}
