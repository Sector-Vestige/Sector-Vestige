// SPDX-FileCopyrightText: 2026 Umbra contributors
// SPDX-FileCopyrightText: 2026 Sector Vestige contributors (modifications)
// SPDX-FileCopyrightText: 2026 ReboundQ3 <22770594+ReboundQ3@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 qu4drivium <aaronholiver@outlook.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Umbra.Examine;

/// <summary>
/// Flavour text when this entity is examined. Set with an action.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedSetExamineSystem))]
public sealed partial class SetExamineComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid? Action;

    [DataField, AutoNetworkedField]
    public EntProtoId ActionPrototype = "ActionSetExtraExamine";

    [DataField, AutoNetworkedField]
    public string ExamineText = string.Empty;
}
