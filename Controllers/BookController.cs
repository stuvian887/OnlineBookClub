using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;

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
    [HttpGet("google")]
    public async Task<IActionResult> GetBookInfoFromGoogle([FromQuery] string isbn)
    {
        var result = await _bookService.GetBookInfoFromGoogleAsync(isbn);
        return result != null
            ? Ok(result)
            : NotFound(new { message = "找不到該 ISBN 的書籍資料。" });
    }

    [HttpPost("{planId}")]
    public async Task<IActionResult> AddBook(int planId, [FromForm] BookDTO bookDto)
    {
        bookDto.platid = planId;
        var result = await _bookService.AddBookAsync(planId, bookDto);

        return result.Item1 != null
            ? Ok(result.Item2)
            : BadRequest(result.Item2);
    }

    [HttpPut("{planId}")]
    public async Task<IActionResult> UpdateBook(int planId, [FromForm] BookDTO bookDto)
    {
        bookDto.platid = planId;
        var result = await _bookService.UpdateBookAsync(planId, bookDto);

        return result.Item1 != null
            ? Ok(result.Item2)
            : BadRequest(result.Item2);
    }
    [HttpGet("bookautoinputurl")]
    public async Task<ActionResult<BookDTO>> GetBookInfo([FromQuery] string url)
    {
        if (string.IsNullOrEmpty(url))
            return BadRequest("URL is required.");

        try
        {
            var info = await _bookService.GetBookInfoAsync(url);
            return Ok(info);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error parsing website: {ex.Message}");
        }
    }
}
