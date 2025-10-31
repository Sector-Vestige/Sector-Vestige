namespace Content.Shared.Destructible;

public abstract class SharedDestructibleSystem : EntitySystem
{
    /// <summary>
    /// Force entity to be destroyed and deleted.
    /// </summary>
<<<<<<< HEAD
    public bool DestroyEntity(EntityUid owner)
=======
    public bool DestroyEntity(Entity<MetaDataComponent?> owner)
>>>>>>> upstream/master
    {
        var ev = new DestructionAttemptEvent();
        RaiseLocalEvent(owner, ev);
        if (ev.Cancelled)
            return false;

        var eventArgs = new DestructionEventArgs();
        RaiseLocalEvent(owner, eventArgs);

<<<<<<< HEAD
        QueueDel(owner);
=======
        PredictedQueueDel(owner);
>>>>>>> upstream/master
        return true;
    }

    /// <summary>
    /// Force entity to break.
    /// </summary>
    public void BreakEntity(EntityUid owner)
    {
        var eventArgs = new BreakageEventArgs();
        RaiseLocalEvent(owner, eventArgs);
    }
}

/// <summary>
<<<<<<< HEAD
///     Raised before an entity is about to be destroyed and deleted
=======
/// Raised before an entity is about to be destroyed and deleted
>>>>>>> upstream/master
/// </summary>
public sealed class DestructionAttemptEvent : CancellableEntityEventArgs
{

}

/// <summary>
<<<<<<< HEAD
///     Raised when entity is destroyed and about to be deleted.
=======
/// Raised when entity is destroyed and about to be deleted.
>>>>>>> upstream/master
/// </summary>
public sealed class DestructionEventArgs : EntityEventArgs
{

}

/// <summary>
/// Raised when entity was heavy damage and about to break.
/// </summary>
public sealed class BreakageEventArgs : EntityEventArgs
{

}
