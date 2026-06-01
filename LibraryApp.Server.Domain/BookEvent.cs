namespace LibraryApp.Server.Domain
{
    public sealed record BookEvent
    {
        public Guid BookEventId { get; init; } = Guid.NewGuid();
        public required Guid BookId { get; init; }
        public BookEventType EventType { get; init; }
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
