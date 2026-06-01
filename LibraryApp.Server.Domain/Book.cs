namespace LibraryApp.Server.Domain
{
    public sealed class Book
    {
        public Guid BookId { get; set; }
        public required string Title { get; set; }
        public required string Owner { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public IList<BookEvent> BookEvents { get; set; } = new List<BookEvent>();
        
        // Verified by E2E tests but Unit tests missing due to reaching time limit
        public bool IsAvailable =>
            !BookEvents.Any() ||
            BookEvents.OrderByDescending(x => x.Timestamp).FirstOrDefault()?.EventType == BookEventType.Return;

    }
}
