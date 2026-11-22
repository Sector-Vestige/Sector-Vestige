using System.Diagnostics.CodeAnalysis;
using Content.Server.Objectives.Components;
using Content.Server.Objectives.Systems;
using Content.Shared._SV.Objectives;
using Content.Shared.Mind;
using Content.Shared.Objectives.Prototypes;
using Content.Shared.Objectives.Systems;
using Robust.Shared.Prototypes;

namespace Content.Server._SV.Objectives;

/// <summary>
/// Lets admins hand out freeform objectives that they author in-round and complete by hand.
/// These are ordinary objective entities built from <see cref="CustomObjectiveProto"/>, whose
/// name and description are overwritten per-assignment. Progress comes from
/// <see cref="CodeConditionComponent"/>, so nothing tracks them automatically.
/// </summary>
public sealed partial class CustomObjectiveSystem : EntitySystem
{
    [Dependency] private IPrototypeManager _proto = default!;
    [Dependency] private CodeConditionSystem _codeCondition = default!;
    [Dependency] private MetaDataSystem _metaData = default!;
    [Dependency] private SharedMindSystem _mind = default!;
    [Dependency] private SharedObjectivesSystem _objectives = default!;

    public static readonly EntProtoId CustomObjectiveProto = "SVCustomObjective";

    /// <summary>
    /// Creates a custom objective with the given title and description and adds it to a mind.
    /// Leaving <paramref name="icon"/> or <paramref name="issuer"/> null keeps whatever the
    /// prototype ships with, which is a paper icon issued by Unknown.
    /// </summary>
    /// <returns>Returns true if the objective was created and added.</returns>
    public bool TryAddCustomObjective(EntityUid mindId, MindComponent mind, string title, string description,
        ProtoId<CustomObjectiveIconPrototype>? icon, ProtoId<ObjectiveIssuerPrototype>? issuer, [NotNullWhen(true)] out EntityUid? objective)
    {
        // goes through the normal creation path so requirement and assign events still run
        objective = _objectives.TryCreateObjective(mindId, mind, CustomObjectiveProto);
        if (objective == null)
            return false;

        _metaData.SetEntityName(objective.Value, title);
        _metaData.SetEntityDescription(objective.Value, description);

        if (icon != null && _proto.TryIndex(icon.Value, out var iconProto))
            _objectives.SetIcon(objective.Value, iconProto.Icon);

        if (issuer != null)
            _objectives.SetIssuer(objective.Value, issuer.Value);

        _mind.AddObjective(mindId, mind, objective.Value);
        return true;
    }

    /// <summary>
    /// Marks one of a mind's objectives complete by its index in <see cref="MindComponent.Objectives"/>.
    /// Only works on objectives backed by <see cref="CodeConditionComponent"/>, which is everything
    /// with no condition of its own to track.
    /// </summary>
    /// <returns>Returns false if the index is out of range or the objective tracks its own progress.</returns>
    public bool TrySetCompleted(MindComponent mind, int index, bool completed = true)
    {
        if (index < 0 || index >= mind.Objectives.Count)
            return false;

        var objective = mind.Objectives[index];
        if (!HasComp<CodeConditionComponent>(objective))
            return false;

        _codeCondition.SetCompleted(objective, completed);
        return true;
    }
}
