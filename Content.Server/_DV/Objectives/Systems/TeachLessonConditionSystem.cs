// SPDX-FileCopyrightText: 2026 Delta-V contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2024 Ermucat <NCramey09@gmail.com>
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReboundQ3 <ReboundQ3@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.DeltaV.Objectives.Components;
using Content.Server.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared.Mind;
using Content.Shared.Mobs;

namespace Content.Server.DeltaV.Objectives.Systems;

/// <summary>
/// Handles teach a lesson condition logic, does not assign target.
/// </summary>
public sealed class TeachLessonConditionSystem : EntitySystem
{
    [Dependency] private readonly CodeConditionSystem _codeCondition = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
    }

    // TODO: subscribe by ref at some point in the future
    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        // Get the mind of the entity that just died (if it has one)
        if (!_mind.TryGetMind(args.Target, out var mindId, out _))
            return;

        // Get all TeachLessonConditionComponent entities
        var query = EntityQueryEnumerator<TeachLessonConditionComponent, TargetObjectiveComponent>();

        while (query.MoveNext(out var uid, out _, out var targetObjective))
        {
            // Check if this objective's target matches the entity that died
            if (targetObjective.Target != mindId)
                continue;

            _codeCondition.SetCompleted(uid);
        }
    }
}
