using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }
        [HttpGet("{planId}")]
        public async Task<IActionResult> GetBookByPlanId(int planId)
        {
            var book = await _bookService.GetBookByPlanIdAsync(planId, Request);

            if (book == null)
                return NotFound(new { message = "找不到該計畫的書籍資料。" });

            return Ok(book);
        }

        [HttpPost("{planId}")]
        public async Task<IActionResult> AddBook(int planId, [FromForm] BookDTO bookDto)
        {
            var result = await _bookService.AddBookAsync(planId, bookDto);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
