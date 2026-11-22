using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Robust.Shared.Serialization;

namespace Content.Shared._SV.Silicon.BorgShutdown;

/// <summary>
/// SV: Action event for toggling borg shutdown state.
/// </summary>
public sealed partial class BorgShutdownActionEvent : InstantActionEvent
{
}

[Serializable, NetSerializable]
public sealed partial class BorgShutdownDoAfterEvent : SimpleDoAfterEvent
{
}
