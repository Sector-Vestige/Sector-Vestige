using Content.Shared.Damage;


// Damages the held item by a set amount when it hits someone. Can be used to make melee items limited-use.
namespace Content.Server.Damage.Components;

[RegisterComponent]
public sealed partial class DamageOnHitComponent : Component
{
    // [DataField("ignoreResistances")] // SV - Armor piercing rework: Begin
    // [ViewVariables(VVAccess.ReadWrite)]
    // public bool IgnoreResistances = true; // SV - Armor piercing rework: End

    [DataField("damage", required: true)]
    [ViewVariables(VVAccess.ReadWrite)]
    public DamageSpecifier Damage = default!;
}

