using Content.Shared._CD.NanoChat;
using Content.Shared.Access.Components;
using Content.Shared.Inventory;
using Content.Shared.Roles.Jobs;
using Content.Shared.StatusIcon;
using Content.Shared.VoiceMask;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Access.Systems;

/// <summary>
/// Handles the UI of the agent ID, as well as automatically updating from voice masks, implants, etc.
/// </summary>
public abstract partial class SharedAgentIdCardSystem : EntitySystem
{
    [Dependency] private SharedIdCardSystem _card = default!;
    [Dependency] private SharedJobSystem _job = default!;
    [Dependency] private SharedJobStatusSystem _jobStatus = default!;
    [Dependency] private SharedNanoChatSystem _nanoChat = default!;

    // SV - Matches maxValue on the NanoChat nameIdentifierGroup prototype.
    private const uint MaxNanoChatNumber = 9999;

    [SubscribeLocalEvent]
    private void OnVoiceMaskNameChanged(Entity<AgentIDCardComponent> ent,
        ref InventoryRelayedEvent<VoiceMaskNameUpdatedEvent> args)
    {
        if (!TryComp<IdCardComponent>(ent, out var idCard))
            return;

        if (!args.Args.VoiceMask.Comp.ChangeIDName)
            return;

        _card.TryChangeFullName(ent, args.Args.NewName, idCard);
    }

    [SubscribeLocalEvent]
    private void OnNameChanged(Entity<AgentIDCardComponent> ent, ref AgentIDCardNameChangedMessage args)
    {
        if (!_card.TryChangeFullName(ent, args.Name))
            return;

        UpdateUi(ent);
    }

    [SubscribeLocalEvent]
    private void OnJobChanged(Entity<AgentIDCardComponent> ent, ref AgentIDCardJobChangedMessage args)
    {
        if (!_card.TryChangeJobTitle(ent, args.Job))
            return;

        UpdateUi(ent);
    }

    [SubscribeLocalEvent]
    private void OnJobIconChanged(Entity<AgentIDCardComponent> ent, ref AgentIDCardJobIconChangedMessage args)
    {
        if (!ProtoMan.Resolve(args.JobIconId, out var jobIcon) ||
            !_card.TryChangeJobIcon(ent, jobIcon))
            return;

        if (_job.TryGetJobFromIcon(jobIcon.ID, out var job))
            _card.TryChangeJobDepartment(ent, job);

        _jobStatus.UpdateIdHolderStatus(ent);
        UpdateUi(ent);
    }

    // SV - Sets the NanoChat number on the card. The handler existed but was left empty, so the
    // number the UI sent was silently discarded and the card kept its original one.
    [SubscribeLocalEvent]
    private void OnNumberChanged(Entity<AgentIDCardComponent> ent, ref AgentIDCardNumberChangedMessage args)
    {
        // The UI already bounds this, but the message is predicted and trivially spoofed.
        if (args.Number is 0 or > MaxNanoChatNumber)
            return;

        if (!TryComp<NanoChatCardComponent>(ent, out var nanoChat))
            return;

        _nanoChat.SetNumber((ent.Owner, nanoChat), args.Number);
        UpdateUi(ent);
    }

    /// <summary>
    /// Update the agent id UI with new component info.
    /// </summary>
    public virtual void UpdateUi(EntityUid entity)
    {
        // Overridden on client
    }
}

/// <summary>
/// Key representing which bound user interface is currently open.
/// Useful when there are multiple UI for an object. Here it's future-proofing only.
/// </summary>
[Serializable, NetSerializable]
public enum AgentIDCardUiKey : byte
{
    Key,
}

/// <summary>
/// Sent from the agent ID UI to change the card name.
/// </summary>
[Serializable, NetSerializable]
public sealed class AgentIDCardNameChangedMessage(string name) : BoundUserInterfaceMessage
{
    public string Name { get; } = name;
}

/// <summary>
/// Sent from the agent ID UI to change the job title.
/// </summary>
[Serializable, NetSerializable]
public sealed class AgentIDCardJobChangedMessage(string job) : BoundUserInterfaceMessage
{
    public string Job { get; } = job;
}

/// <summary>
/// Sent from the agent ID UI to change the job icon.
/// </summary>
[Serializable, NetSerializable]
public sealed class AgentIDCardJobIconChangedMessage(ProtoId<JobIconPrototype> icon) : BoundUserInterfaceMessage
{
    public ProtoId<JobIconPrototype> JobIconId { get; } = icon;
}

/// <summary>
/// CD System fixed by Sector Vestige
/// Sent from the agent ID UI to change the Nanochat number
/// </summary>
[Serializable, NetSerializable]
public sealed class AgentIDCardNumberChangedMessage(uint number) :  BoundUserInterfaceMessage
{
    public uint Number { get; } = number;
}
