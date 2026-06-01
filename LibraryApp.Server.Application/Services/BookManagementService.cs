using LibraryApp.Server.Application.Interfaces;
using LibraryApp.Server.Application.Interfaces.Repositories;
using LibraryApp.Server.Domain;
using Microsoft.Extensions.Logging;

namespace LibraryApp.Server.Application.Services
{
    public class BookManagementService : IBookManagementService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookEventRepository _bookEventRepository;
        private readonly ILogger<BookManagementService> _logger;

        public BookManagementService(
            IBookRepository bookRepository,
            IBookEventRepository bookEventRepository,
            ILogger<BookManagementService> logger)
        {
            _bookRepository = bookRepository;
            _bookEventRepository = bookEventRepository;
            _logger = logger;
        }

        public async Task<Book?> BorrowBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookRepository.GetAsync(bookId, cancellationToken);

                if (book is not null)
                {
                    var borrowEvent = new BookEvent { BookId = bookId, EventType = BookEventType.Borrow };

                    await _bookEventRepository.CreateAsync(borrowEvent, cancellationToken);

                    book.BookEvents.Add(borrowEvent);
                }

                return book;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookManagementService), nameof(BorrowBookAsync));
                throw;
            }
        }

        public async Task<Book> CreateBookAsync(Book book, CancellationToken cancellationToken)
        {
            return await _bookRepository.CreateAsync(book, cancellationToken);
        }

        public async Task<Book?> GetBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            return await _bookRepository.GetAsync(bookId, cancellationToken);
        }

        public async Task<IEnumerable<Book>> GetBooksAsync(CancellationToken cancellationToken)
        {
            return await _bookRepository.GetAllActiveAsync(cancellationToken);
        }

        public async Task<bool> RemoveBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            return await _bookRepository.DeleteAsync(bookId, cancellationToken);
        }

        public async Task<Book?> ReturnBookAsync(Guid bookId, CancellationToken cancellationToken)
        {
            try
            {
                var book = await _bookRepository.GetAsync(bookId, cancellationToken);

                if (book is not null)
                {
                    var returnEvent = new BookEvent { BookId = bookId, EventType = BookEventType.Return };

                    await _bookEventRepository.CreateAsync(returnEvent, cancellationToken);

                    book.BookEvents.Add(returnEvent);
                }

                return book;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookManagementService), nameof(ReturnBookAsync));
                throw;
            }
        }

        public async Task<Book?> UpdateBookAsync(Book book, CancellationToken cancellationToken)
        {
            return await _bookRepository.UpdateAsync(book, cancellationToken);
        }
    }
}
