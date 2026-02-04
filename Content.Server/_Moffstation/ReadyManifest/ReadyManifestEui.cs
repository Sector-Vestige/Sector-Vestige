// SPDX-FileCopyrightText: 2026 Moffstation contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
// SPDX-FileCopyrightText: 2025 vestige-bot <vestige-bot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.EUI;
using Content.Shared._Moffstation.ReadyManifest;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._Moffstation.ReadyManifest;

public sealed class ReadyManifestEui(ReadyManifestSystem readyManifestSystem) : BaseEui
{
    public override ReadyManifestEuiState GetNewState()
    {
        var entries = new Dictionary<ProtoId<JobPrototype>, int>(readyManifestSystem.GetReadyManifest());
        return new ReadyManifestEuiState(entries);
    }

    public override void Closed()
    {
        readyManifestSystem.CloseEui(Player);
    }
}
