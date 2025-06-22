using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Service;
using OnlineBookClub.Services;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearnController : ControllerBase
    {
        private readonly LearnService _service;
        private readonly NoticeService _noticeService; // 通知服務
        private readonly BookPlanService _bookPlanService;  
        public LearnController(LearnService service, NoticeService noticeService, BookPlanService bookPlanService)
        {
            _service = service;
            _noticeService = noticeService;
            _bookPlanService = bookPlanService;
        }

        //// GET: api/<LearnController>
        //[HttpGet("Index")]
        //public async Task<IActionResult> GetAllLearn()
        //{
        //    return Ok(await _service.GetAllLearn()) ;
        //}

        // GET api/<LearnController>/5
        [HttpGet("Index/{Chapter_Id}")]
        public async Task<IActionResult> Get(int Chapter_Id)
        {
            int UserId = GetUser();
            return Ok(await _service.GetLearnByChapter_Id(UserId , Chapter_Id));
        }
        [HttpGet("Calendar")]
        public async Task<IActionResult> GetLearnByCalendar([FromQuery] DateTime? BeginTime , [FromQuery] DateTime? EndTime)
        {
            int UserId = GetUser();
            var result = await _service.GetLearnByCalendar(UserId, BeginTime, EndTime);
            
            if(result.Item1 != null)
            {
               
                return Ok(result.Item1);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // POST api/<LearnController>
        [HttpPost("Create/{Chapter_Id}")]
        public async Task<IActionResult> CreateLearn(int Chapter_Id, [FromBody] LearnDTO newData)
        {
            int UserId = GetUser();
            var result = await _service.CreateLearn(UserId, Chapter_Id , newData);

            if (result.Item1 != null)
            { // 創建通知
              

                return Ok(new { message = result.Message });
            }
            else
            {
                return BadRequest(new { message = result.Message });
            }
        }

        // PUT api/<LearnController>/5
        [HttpPut("Update/{Chapter_Id}/{Learn_Index}/")]
        public async Task<IActionResult> UpdateLearn(int Chapter_Id, int Learn_Index , [FromBody] LearnDTO updateData)
        {
            int UserId = GetUser();
            var result = await _service.UpdateLearn(UserId, Chapter_Id, Learn_Index, updateData);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // DELETE api/<LearnController>/5
        [HttpDelete("Delete/{Chapter_Id}/{Learn_Index}")]
        public async Task<IActionResult> Delete(int Chapter_Id , int Learn_Index)
        {
            int UserId = GetUser();
            var result = await _service.DeleteLearn(UserId, Chapter_Id, Learn_Index);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpGet("GetMemberPassPersent/{PlanId}")]
        public async Task<IActionResult> GetMemberPassPersent(int PlanId)
        {
            int UserId = GetUser();
            var result = await _service.GetMemberProgressAsync(UserId , PlanId);
            if (result != null)
            {
                return Ok(result);
            }
            else { return BadRequest(new { message = "發生錯誤" }); }
        }
        [HttpGet("Answer_Record/{Chapter_Id}/{Learn_Index}")]
        public async Task<IActionResult> GetAnswer_Record(int Chapter_Id, int Learn_Index)
        {
            int UserId = GetUser();
            return Ok(await _service.GetAnswer_Record(UserId, Chapter_Id, Learn_Index));
        }
        [HttpPost("Answer_Record")]
        public async Task<IActionResult> CreateAnswer_Record([FromBody] AnswerSubmissionDTO Answer)
        {
            int UserId = GetUser();
            var result = await _service.CreateAnswer_Record(UserId, Answer);
            if(result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }
        [HttpPut("PassProgress/{Chapter_Id}/{Learn_Index}")]
        public async Task<IActionResult> PassProgress(int Chapter_Id, int Learn_Index)
        {
            int UserId = GetUser();
            var result = await _service.PassProgressAsync(UserId, Chapter_Id, Learn_Index);
            if(result != null)
            {
                return Ok(new { message = "進度通過!" });
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }

        [HttpGet("GetMemberByLearn/{Chapter_Id}/{Learn_Index}")]
        public async Task<IActionResult> GetMemberByLearn(int Chapter_Id, int Learn_Index)  
        {
            int UserId = GetUser();
            var result = await _service.GetMemberByLearn(UserId, Chapter_Id, Learn_Index);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }

        private int GetUser()
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("發生異常錯誤，找不到登入的人是誰"); ;
            return int.Parse(UserIdClaim.Value);
        }
        [HttpPost("MoveLearnToChapter")]
        public void MoveLearnToChapter()
        {
            _service.MoveLearnToChapter();
        }
    }
}


