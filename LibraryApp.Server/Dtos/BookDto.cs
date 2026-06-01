using LibraryApp.Server.Domain;
using Newtonsoft.Json;

namespace LibraryApp.Server.Dtos
{
    public record BookDto
    {
        [JsonProperty("key")]
        public Guid Key { get; init; } = Guid.Empty;
        [JsonProperty("bookId")]
        public Guid BookId { get; init; } = Guid.Empty;
        [JsonProperty("title")]
        public required string Title { get; init; }
        [JsonProperty("owner")]
        public required string Owner { get; init; }
        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; init; }

        public Book ToModel(Guid bookId)
        {
            return new Book
            {
                BookId = BookId == Guid.Empty ? bookId : BookId,
                Title = Title,
                Owner = Owner,
            };
        }
    }
}
