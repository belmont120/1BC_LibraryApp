using LibraryApp.Server.Domain;

namespace LibraryApp.Server.Application.Interfaces.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllActiveAsync(CancellationToken cancellationToken);
        Task<Book?> GetAsync(Guid bookId, CancellationToken cancellationToken);
        Task<Book> CreateAsync(Book book, CancellationToken cancellationToken);
        Task<Book?> UpdateAsync(Book book, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid bookId, CancellationToken cancellationToken);
    }
}
