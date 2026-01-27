// SPDX-FileCopyrightText: 2026 Sector-Vestige contributors
// SPDX-FileCopyrightText: 2026 qu4drivium <aaronholiver@outlook.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Traits.Assorted;
using Content.Shared.EntityEffects;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared._CD.EntityEffects.Effects;

[UsedImplicitly]
public sealed partial class ResetParacusia : EntityEffectBase<ResetParacusia>
{
    [DataField("TimerReset")]
    public int TimerReset = 600;

     public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
        => Loc.GetString("reagent-effect-guidebook-reset-paracusia", ("chance", Probability));
}
