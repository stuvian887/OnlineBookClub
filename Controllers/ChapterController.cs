using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using OnlineBookClub.Services;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly ChapterService _service;
        private readonly NoticeService _noticeService; // 通知服務
        private readonly BookPlanService _bookPlanService;
        public ChapterController(ChapterService service, NoticeService noticeService, BookPlanService bookPlanService)
        {
            _service = service;
            _noticeService = noticeService;
            _bookPlanService = bookPlanService;
        }
        // GET: api/<ChapterController>
        [HttpGet("GetChapterByPlan/{Plan_Id}")]
        public async Task<IActionResult> GetChapter(int Plan_Id)
        {
            var result = await _service.GetChapter(Plan_Id);
            return Ok(result);
        }
        [HttpPost("CreateChapter/{Plan_Id}")]
        public async Task<IActionResult> CreateChapter(int Plan_Id, [FromBody] ChapterDTO newData)
        {
            int UserId = GetUser();
            string result = await _service.CreateChapter(UserId, Plan_Id, newData);
            if (result == "Success")
            {
                return Ok(new { message = "新增章節成功" });
            }
            else
            {
                return BadRequest(result);
            }
            
        }
        [HttpPut("UpdateChapter/{Chapter_Id}")]
        public async Task<IActionResult> UpdateChapter(int Chapter_Id, [FromBody] ChapterDTO updateData)
        {
            int UserId = GetUser();
            var result = await _service.UpdateChapter(UserId, Chapter_Id, updateData);
            if(result.Item1 == null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpDelete("DeleteChapter/{Chapter_Id}")]
        public async Task<IActionResult> DeleteChapter(int Chapter_Id)
        {
            int UserId = GetUser();
            string result = await _service.DeleteChapter(UserId, Chapter_Id);
            if(result != "刪除成功")
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        private int GetUser()
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("發生異常錯誤，找不到登入的人是誰"); ;
            return int.Parse(UserIdClaim.Value);
        }
    }
}
