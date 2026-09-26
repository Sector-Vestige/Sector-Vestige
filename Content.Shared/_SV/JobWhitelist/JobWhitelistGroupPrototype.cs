using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Shared._SV.JobWhitelist;

/// <summary>
/// Defines a group of jobs that can be whitelisted together.
/// For example, a "trusted" group might include all head roles.
/// </summary>
[Prototype]
public sealed partial class JobWhitelistGroupPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    /// <summary>
    /// Human-readable name for this whitelist group.
    /// </summary>
    [DataField(required: true)]
    public string Name = string.Empty;

    /// <summary>
    /// Description of what this group grants access to.
    /// </summary>
    [DataField]
    public string Description = string.Empty;

    /// <summary>
    /// List of jobs that this group grants access to.
    /// </summary>
    [DataField(required: true)]
    public List<ProtoId<JobPrototype>> Jobs = new();
}
