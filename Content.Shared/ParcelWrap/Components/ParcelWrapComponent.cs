using Content.Shared.Item;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.ParcelWrap.Components;

/// <summary>
/// This component gives its owning entity the ability to wrap items into parcels.
/// </summary>
/// <seealso cref="Components.WrappedParcelComponent"/>
<<<<<<< HEAD
[RegisterComponent, NetworkedComponent]
=======
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
>>>>>>> upstream/master
[Access] // Readonly, except for VV editing
public sealed partial class ParcelWrapComponent : Component
{
    /// <summary>
    /// The <see cref="EntityPrototype"/> of the parcel created by using this component.
    /// </summary>
<<<<<<< HEAD
    [DataField(required: true)]
=======
    [DataField(required: true), AutoNetworkedField]
>>>>>>> upstream/master
    public EntProtoId ParcelPrototype;

    /// <summary>
    /// If true, parcels created by this will have the same <see cref="ItemSizePrototype">size</see> as the item they
    /// contain. If false, parcels created by this will always have the size specified by <see cref="FallbackItemSize"/>.
    /// </summary>
<<<<<<< HEAD
    [DataField]
=======
    [DataField, AutoNetworkedField]
>>>>>>> upstream/master
    public bool WrappedItemsMaintainSize = true;

    /// <summary>
    /// The <see cref="ItemSizePrototype">size</see> of parcels created by this component's entity. This is used if
    /// <see cref="WrappedItemsMaintainSize"/> is false, or if the item being wrapped somehow doesn't have a size.
    /// </summary>
<<<<<<< HEAD
    [DataField]
=======
    [DataField, AutoNetworkedField]
>>>>>>> upstream/master
    public ProtoId<ItemSizePrototype> FallbackItemSize = "Ginormous";

    /// <summary>
    /// If true, parcels created by this will have the same shape as the item they contain. If false, parcels created by
    /// this will have the default shape for their size.
    /// </summary>
<<<<<<< HEAD
    [DataField]
=======
    [DataField, AutoNetworkedField]
>>>>>>> upstream/master
    public bool WrappedItemsMaintainShape;

    /// <summary>
    /// How long it takes to use this to wrap something.
    /// </summary>
<<<<<<< HEAD
    [DataField(required: true)]
=======
    [DataField(required: true), AutoNetworkedField]
>>>>>>> upstream/master
    public TimeSpan WrapDelay = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Sound played when this is used to wrap something.
    /// </summary>
<<<<<<< HEAD
    [DataField]
=======
    [DataField, AutoNetworkedField]
>>>>>>> upstream/master
    public SoundSpecifier? WrapSound;

    /// <summary>
    /// Defines the set of things which can be wrapped (unless it fails the <see cref="Blacklist"/>).
    /// </summary>
<<<<<<< HEAD
    [DataField]
=======
    [DataField, AutoNetworkedField]
>>>>>>> upstream/master
    public EntityWhitelist? Whitelist;

    /// <summary>
    /// Defines the set of things which cannot be wrapped (even if it passes the <see cref="Whitelist"/>).
    /// </summary>
<<<<<<< HEAD
    [DataField]
    public EntityWhitelist? Blacklist;
=======
    [DataField, AutoNetworkedField]
    public EntityWhitelist? Blacklist;

    /// <summary>
    /// If a player trapped inside this parcel can escape from it by unwrapping it.
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool CanSelfUnwrap = true;
>>>>>>> upstream/master
}
