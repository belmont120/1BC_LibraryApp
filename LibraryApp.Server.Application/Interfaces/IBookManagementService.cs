using LibraryApp.Server.Domain;

namespace LibraryApp.Server.Application.Interfaces
{
    public interface IBookManagementService
    {
        Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken);
        Task<Book?> GetBookAsync(Guid bookId, CancellationToken cancellationToken);
        Task<Book?> UpdateBookAsync(Book book, CancellationToken cancellationToken);
        Task<bool> RemoveBookAsync(Guid bookId, CancellationToken cancellationToken);
        Task<Book?> BorrowBookAsync(Guid bookId, CancellationToken cancellationToken);
        Task<Book?> ReturnBookAsync(Guid bookId, CancellationToken cancellationToken);
        Task<Book> CreateBookAsync(Book book, CancellationToken cancellationToken);
    }
}
