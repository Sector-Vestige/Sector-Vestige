namespace Content.Shared.Speech
{
    public sealed class SpeakAttemptEvent : CancellableEntityEventArgs
    {
        public SpeakAttemptEvent(EntityUid uid, bool isWhisper = false)
        {
            Uid = uid;
            IsWhisper = isWhisper;
        }

        public EntityUid Uid { get; }
        public bool IsWhisper { get; }
    }
}
