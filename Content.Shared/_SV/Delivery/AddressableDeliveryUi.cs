using Robust.Shared.Serialization;

namespace Content.Shared._SV.Delivery;

/// <summary>
/// UI key for the player-addressable letter window.
/// </summary>
[Serializable, NetSerializable]
public enum DeliveryAddressUiKey
{
    Key,
}

/// <summary>
/// State for the player-addressable letter UI.
/// Lists the station crew the letter can be addressed to.
/// </summary>
[Serializable, NetSerializable]
public sealed class AddressableDeliveryBuiState : BoundUserInterfaceState
{
    /// <summary>
    /// Station record id -> display label (name and job) for each addressable crewmember.
    /// </summary>
    public readonly Dictionary<uint, string> Crew;

    /// <summary>
    /// Currently selected record id, if any.
    /// </summary>
    public readonly uint? SelectedKey;

    public AddressableDeliveryBuiState(Dictionary<uint, string> crew, uint? selectedKey)
    {
        Crew = crew;
        SelectedKey = selectedKey;
    }
}

/// <summary>
/// Sent from the client when the player picks a crewmember to address the letter to.
/// </summary>
[Serializable, NetSerializable]
public sealed class AddressableDeliverySelectMessage : BoundUserInterfaceMessage
{
    /// <summary>
    /// Station record id of the chosen recipient.
    /// </summary>
    public readonly uint RecordKey;

    public AddressableDeliverySelectMessage(uint recordKey)
    {
        RecordKey = recordKey;
    }
}

/// <summary>
/// Sent from the client when the player sends the addressed letter through the mail system.
/// </summary>
[Serializable, NetSerializable]
public sealed class AddressableDeliverySendMessage : BoundUserInterfaceMessage
{
}
