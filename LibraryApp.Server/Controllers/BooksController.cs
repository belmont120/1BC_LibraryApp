using LibraryApp.Server.Application.Interfaces;
using LibraryApp.Server.Dtos;
using LibraryApp.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LibraryApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookManagementService _bookManagementService;
        private readonly ILogger<BooksController> _logger;
        public BooksController(IBookManagementService bookManagementService, ILogger<BooksController> logger)
        {
            _bookManagementService = bookManagementService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
        {
            var books = await _bookManagementService.GetBooksAsync(cancellationToken);

            return new OkObjectResult(books.Select(x => x.ToDto()));
        }

        [HttpGet("{bookId}", Name = "GetBookByIdAsync")]
        public async Task<IActionResult> GetBookByIdAsync(Guid bookId, CancellationToken cancellationToken)
        {
            if (bookId == Guid.Empty)
            {
                return new BadRequestObjectResult("BookId cannot be empty.");
            }

            var book = await _bookManagementService.GetBookAsync(bookId, cancellationToken);

            if (book is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(book.ToDto());
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] BookDto dto, CancellationToken cancellationToken)
        {
            if (dto is null)
            {
                return new BadRequestResult();
            }

            var book = await _bookManagementService.CreateBookAsync(dto.ToModel(Guid.NewGuid()), cancellationToken);

            return new CreatedAtRouteResult(
                routeName: "GetBookByIdAsync",
                routeValues: new { bookId = book.BookId },
                value: book.ToDto());

        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> PutAsync(Guid bookId, [FromBody] string payload, CancellationToken cancellationToken)
        {
            if (bookId == Guid.Empty)
            {
                return new BadRequestObjectResult("BookId cannot be empty.");
            }

            var dto = JsonConvert.DeserializeObject<BookDto>(payload);

            if (dto is null)
            {
                return new BadRequestObjectResult("Invalid request payload.");
            }

            var book = await _bookManagementService.UpdateBookAsync(dto.ToModel(bookId), cancellationToken);

            if (book is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(book.ToDto());
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteAsync(Guid bookId, CancellationToken cancellationToken)
        {
            if (bookId == Guid.Empty)
            {
                return new BadRequestObjectResult("BookId cannot be empty.");
            }

            var isDeleted = await _bookManagementService.RemoveBookAsync(bookId, cancellationToken);

            if (!isDeleted)
            {
                return new NotFoundResult();
            }

            return new OkResult();
        }

        [HttpPost("{bookId}/borrow")]
        public async Task<IActionResult> BorrowAsync(Guid bookId, CancellationToken cancellationToken)
        {
            if (bookId == Guid.Empty)
            {
                return new BadRequestObjectResult("BookId cannot be empty.");
            }

            var book = await _bookManagementService.BorrowBookAsync(bookId, cancellationToken);

            if (book is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(book.ToDto());
        }

        [HttpPost("{bookId}/return")]
        public async Task<IActionResult> ReturnAsync(Guid bookId, CancellationToken cancellationToken)
        {
            if (bookId == Guid.Empty)
            {
                return new BadRequestObjectResult("BookId cannot be empty.");
            }

            var book = await _bookManagementService.ReturnBookAsync(bookId, cancellationToken);

            if (book is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(book.ToDto());
        }
    }
}
