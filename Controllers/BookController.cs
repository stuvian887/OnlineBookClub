using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using System.Numerics;

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
        public async Task<IActionResult> AddBook(int planId,[FromBody]BookDTO bookDto)
        {
            bookDto.platid = planId;
            var result = await _bookService.AddBookAsync(bookDto.platid, bookDto);
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
