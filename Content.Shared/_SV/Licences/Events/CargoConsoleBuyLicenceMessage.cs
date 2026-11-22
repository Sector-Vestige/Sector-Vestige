using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.Licences.Events;

/// <summary>
///     SV - Request to buy a licence from the cargo console.
/// </summary>
[Serializable, NetSerializable]
public sealed class CargoConsoleBuyLicenceMessage : BoundUserInterfaceMessage
{
    public ProtoId<LicencePrototype> Licence;

    public CargoConsoleBuyLicenceMessage(ProtoId<LicencePrototype> licence)
    {
        Licence = licence;
    }
}
