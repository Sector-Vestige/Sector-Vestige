namespace Content.Server._Umbra.Power.Components;

[RegisterComponent]
public sealed partial class ElectricalOverloadComponent : Component
{
    [DataField]
    public string ExplosionOnOverload = "Default";

    [ViewVariables]
    public DateTime ExplodeAt = DateTime.MaxValue;

    [ViewVariables]
    public DateTime NextBuzz = DateTime.MaxValue;
}
