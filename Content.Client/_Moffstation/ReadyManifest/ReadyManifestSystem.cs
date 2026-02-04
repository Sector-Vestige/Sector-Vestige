// SPDX-FileCopyrightText: 2026 Moffstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
// SPDX-FileCopyrightText: 2025 vestige-bot <vestige-bot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared._Moffstation.ReadyManifest;

namespace Content.Client._Moffstation.ReadyManifest;

public sealed class ReadyManifestSystem : EntitySystem
{
    public void RequestReadyManifest()
    {
        RaiseNetworkEvent(new RequestReadyManifestMessage());
    }
}
