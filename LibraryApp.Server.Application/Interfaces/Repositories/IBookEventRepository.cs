using LibraryApp.Server.Domain;

namespace LibraryApp.Server.Application.Interfaces.Repositories
{
    public interface IBookEventRepository
    {
        Task CreateAsync(BookEvent bookEvent, CancellationToken cancellationToken);
    }
}
