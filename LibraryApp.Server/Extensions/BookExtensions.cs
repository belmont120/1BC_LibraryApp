using LibraryApp.Server.Domain;
using LibraryApp.Server.Dtos;

namespace LibraryApp.Server.Extensions
{
    public static class BookExtensions
    {
        public static BookDto ToDto(this Book book)
        {
            return new BookDto
            {
                Key = book.BookId,
                BookId = book.BookId,
                Title = book.Title,
                Owner = book.Owner,
                IsAvailable = book.IsAvailable,
            };
        }
    }
}
