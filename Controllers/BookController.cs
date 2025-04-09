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
            var book = await _bookService.GetBookByPlanIdAsync(planId);

            if (book == null)
                return NotFound(new { message = "找不到該計畫的書籍資料。" });

            return Ok(book);
        }

        [HttpPost("{planId}")]
        public async Task<IActionResult> AddBook(int planId, [FromBody] BookDTO bookDto)
        {
            await _bookService.AddBookAsync(planId, bookDto);
            return Ok(new { message = "書籍儲存成功！" });
        }
    }
}
