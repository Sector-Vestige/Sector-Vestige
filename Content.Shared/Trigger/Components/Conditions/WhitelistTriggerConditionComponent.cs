using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.Trigger.Components.Conditions;

/// <summary>
<<<<<<< HEAD
/// Checks if the user of a trigger satisfies a whitelist and blacklist condition for the triggered entity or the one triggering it.
=======
/// Checks if the user of a trigger satisfies a whitelist and blacklist condition.
>>>>>>> upstream/master
/// Cancels the trigger otherwise.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WhitelistTriggerConditionComponent : BaseTriggerConditionComponent
{
    /// <summary>
<<<<<<< HEAD
    /// Whitelist for what entites can cause this trigger.
=======
    /// Whitelist for what entities can cause this trigger.
>>>>>>> upstream/master
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? UserWhitelist;

    /// <summary>
<<<<<<< HEAD
    /// Blacklist for what entites can cause this trigger.
=======
    /// Blacklist for what entities can cause this trigger.
>>>>>>> upstream/master
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? UserBlacklist;
}
