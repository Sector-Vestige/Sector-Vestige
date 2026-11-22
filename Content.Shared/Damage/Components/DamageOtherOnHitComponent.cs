using Content.Shared.Damage.Systems;

namespace Content.Shared.Damage.Components;

/// <summary>
/// Makes this entity deal damage when thrown at something.
/// </summary>
[RegisterComponent]
[Access(typeof(SharedDamageOtherOnHitSystem))]
public sealed partial class DamageOtherOnHitComponent : Component
{
    // /// <summary> // SV - Armor piercing rework: Begin
    // /// Whether to ignore damage modifiers.
    // /// </summary>
    // [DataField]
    // public bool IgnoreResistances = false; // SV - Armor piercing rework: Begin

    /// <summary>
    /// The damage amount to deal on hit.
    /// </summary>
    [DataField(required: true)]
    public DamageSpecifier Damage = default!;

}
