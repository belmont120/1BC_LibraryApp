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
    public class BookEventRepository : IBookEventRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<BookEventRepository> _logger;

        public BookEventRepository(IOptions<ConnectionStringsOptions> options, ILogger<BookEventRepository> logger)
        {
            _connectionString = options.Value.DefaultConnection;
            _logger = logger;
        }

        public async Task CreateAsync(BookEvent bookEvent, CancellationToken cancellationToken)
        {
            try
            {
                var sql = @"
                    INSERT INTO BookEvents (BookEventId, BookId, EventType, Timestamp) 
                    VALUES(@bookEventId, @bookId, @eventType, @timestamp)";

                var command = new CommandDefinition(sql,
                    new
                    {
                        bookEventId = bookEvent.BookEventId,
                        bookId = bookEvent.BookId,
                        eventType = bookEvent.EventType,
                        timestamp = bookEvent.Timestamp
                    },
                    cancellationToken: cancellationToken);

                await using var connection = new SqlConnection(_connectionString);

                var rowsAffected = await connection.ExecuteAsync(command);

                if (rowsAffected != 1)
                {
                    throw new InvalidOperationException($"Create new BookEvent failed. {JsonConvert.SerializeObject(bookEvent)}");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{ServiceName} {Method} encountered an error.", nameof(BookEventRepository), nameof(CreateAsync));
                throw;
            }
        }
    }
}
