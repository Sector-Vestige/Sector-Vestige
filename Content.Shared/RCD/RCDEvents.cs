using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.RCD;

[Serializable, NetSerializable]
public sealed class RCDSystemMessage(ProtoId<RCDPrototype> protoId) : BoundUserInterfaceMessage
{
    public ProtoId<RCDPrototype> ProtoId = protoId;
}

[Serializable, NetSerializable]
public sealed class RCDConstructionGhostRotationEvent(NetEntity netEntity, Direction direction) : EntityEventArgs
{
    public readonly NetEntity NetEntity = netEntity;
    public readonly Direction Direction = direction;
}

//Sector Vestige - Begin: Logic for an RCD or RPD to have a flipped prototype that one can flip between
[Serializable, NetSerializable]
public sealed class RCDConstructionGhostFlipEvent(NetEntity netEntity) : EntityEventArgs
{
    public readonly NetEntity NetEntity = netEntity;
}
//Sector Vestige - End: Logic for an RCD or RPD to have a flipped prototype that one can flip between

[Serializable, NetSerializable]
public enum RcdUiKey : byte
{
    Key
}
