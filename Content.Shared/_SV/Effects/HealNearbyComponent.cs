using Content.Shared.Damage;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._SV.Effects;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentPause, AutoGenerateComponentState]
public sealed partial class HealNearbyComponent : Component
{
    /// <summary>
    /// Damage to apply to entities that are strapped to this entity.
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier Damage = null!;

    /// <summary>
    /// How frequently the damage should be applied, in seconds.
    /// </summary>
    [DataField(required: false)]
    public TimeSpan HealTime = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Next time that <see cref="Damage"/> will be applied.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoPausedField, AutoNetworkedField]
    public TimeSpan NextHealTime = TimeSpan.Zero; //Next heal

    /// <summary>
    /// The radius that the healing will apply
    /// </summary>
    [DataField]
    public float Radius;
}
