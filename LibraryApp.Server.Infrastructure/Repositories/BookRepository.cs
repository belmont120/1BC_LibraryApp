using Dapper;
using LibraryApp.Server.Application.Interfaces.Repositories;
using LibraryApp.Server.Application.Options;
using LibraryApp.Server.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LibraryApp.Server.Infrastructure.Repositories
{
    public sealed class BookRepository : IBookRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<BookRepository> _logger;

        public BookRepository(IOptions<ConnectionStringsOptions> options, ILogger<BookRepository> logger)
        {
            _connectionString = options.Value.DefaultConnection;
            _logger = logger;
        }

        public async Task<Book> CreateAsync(Book book, CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"
                    INSERT INTO Books (BookId, Title, Owner, CreatedAt) 
                    VALUES(@bookId, @title, @owner, @createdAt)";

                if (book.BookId == Guid.Empty)
                {
                    book.BookId = Guid.NewGuid();
                }

                var command = new CommandDefinition(sql,
                    new
                    {
                        bookId = book.BookId,
                        title = book.Title,
                        owner = book.Owner,
                        createdAt = book.CreatedAt
                    },
                    cancellationToken: cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var rowsAffected = await connection.ExecuteAsync(command);

                if (rowsAffected != 1)
                {
                    throw new InvalidOperationException($"Create new Book failed. {JsonConvert.SerializeObject(book)}");
                }

                return book;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookRepository), nameof(CreateAsync));
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid bookId, CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"
                    UPDATE Books 
                    SET IsDeleted = 1, 
                        DeletedAt = @deletedAt 
                    WHERE BookId = @bookId";

                var command = new CommandDefinition(sql,
                    new { bookId, deletedAt = DateTime.UtcNow }, cancellationToken: cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var rowsAffected = await connection.ExecuteAsync(command);

                if (rowsAffected == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookRepository), nameof(DeleteAsync));
                throw;
            }
        }

        public async Task<IEnumerable<Book>> GetAllActiveAsync(CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"
                    SELECT B.BookId, B.Title, B.Owner, B.CreatedAt, B.UpdatedAt, 
                        BV.BookEventId, BV.EventType, BV.Timestamp
                    FROM Books B
                    LEFT JOIN BookEvents BV ON BV.BookId = B.BookId
                    WHERE B.IsDeleted = 0
                    ORDER BY B.Title";

                var bookDictionary = new Dictionary<Guid, Book>();

                var command = new CommandDefinition(sql, cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var books = (await connection.QueryAsync<Book, BookEvent, Book>(command,
                    (book, bookEvent) =>
                    {
                        if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                        {
                            currentBook = book;
                            currentBook.BookEvents = new List<BookEvent>();
                            bookDictionary.Add(currentBook.BookId, currentBook);
                        }

                        if (bookEvent != null)
                        {
                            currentBook.BookEvents.Add(bookEvent);
                        }

                        return currentBook;
                    }, splitOn: nameof(BookEvent.BookEventId)))
                    .Distinct().ToList();

                return books;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookRepository), nameof(GetAllActiveAsync));
                throw;
            }
        }

        public async Task<Book?> GetAsync(Guid bookId, CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"
                    SELECT B.BookId, B.Title, B.Owner, B.CreatedAt, B.UpdatedAt 
                    FROM Books B 
                    WHERE B.BookId = @bookId AND B.IsDeleted = 0";

                var command = new CommandDefinition(sql, new { bookId }, cancellationToken: cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var book = await connection.QuerySingleOrDefaultAsync<Book>(command);

                if (book is not null)
                {
                    book.BookEvents = (await GetBookEventsAsync(bookId, cancellationToken)).ToList();
                }

                return book;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookRepository), nameof(DeleteAsync));
                throw;
            }
        }

        private async Task<IEnumerable<BookEvent>> GetBookEventsAsync(Guid bookId, CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"
                    SELECT BV.BookEventId, BV.BookId, BV.EventType, BV.Timestamp
                    FROM BookEvents BV 
                    WHERE BV.BookId = @bookId";

                var command = new CommandDefinition(sql, new { bookId }, cancellationToken: cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var bookEvents = await connection.QueryAsync<BookEvent>(command);

                return bookEvents;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookRepository), nameof(GetBookEventsAsync));
                throw;
            }
        }

        public async Task<Book?> UpdateAsync(Book book, CancellationToken cancellationToken)
        {
            try
            {
                book.UpdatedAt = DateTime.UtcNow;

                var sql = @"
                    UPDATE Books 
                    SET Title = @title, 
                        Owner = @owner, 
                        UpdatedAt = @updatedAt
                    WHERE BookId = @bookId
                    AND IsDeleted = 0";

                var command = new CommandDefinition(sql,
                    new { bookId = book.BookId, title = book.Title, owner = book.Owner, updatedAt = book.UpdatedAt },
                    cancellationToken: cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var rowsAffected = await connection.ExecuteAsync(command);

                if (rowsAffected == 0)
                {
                    return null;
                }

                return book;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookRepository), nameof(UpdateAsync));
                throw;
            }
        }
    }
}
