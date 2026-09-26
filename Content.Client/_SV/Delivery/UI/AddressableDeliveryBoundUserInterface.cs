using Content.Shared._SV.Delivery;
using Robust.Client.UserInterface;

namespace Content.Client._SV.Delivery.UI;

/// <summary>
/// Opens the <see cref="AddressableDeliveryWindow"/> for a player-addressable letter, feeds it the
/// station crew listing pushed from the server, and relays the player's choices back to the server.
/// </summary>
public sealed partial class AddressableDeliveryBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private AddressableDeliveryWindow? _window;

    public AddressableDeliveryBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<AddressableDeliveryWindow>();
        _window.OnRecipientSelected += OnRecipientSelected;
        _window.OnSendPressed += OnSendPressed;
    }

    private void OnRecipientSelected(uint recordKey)
    {
        SendMessage(new AddressableDeliverySelectMessage(recordKey));
    }

    private void OnSendPressed()
    {
        SendMessage(new AddressableDeliverySendMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is AddressableDeliveryBuiState cast)
            _window?.UpdateState(cast);
    }
}
