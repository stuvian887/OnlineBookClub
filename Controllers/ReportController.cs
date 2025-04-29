using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Service;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Claims;
using System.Xml.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportService _service;
        public ReportController(ReportService service)
        {
            _service = service;
        }

        // GET: api/<ReportController>
        [HttpGet("Post/{P_Report_Id}")]
        public async Task<IActionResult> GetPostReport(int P_Report_Id)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetPostReport(UserId, P_Report_Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }
        [HttpGet("GetAllReport/{PlanId}")]
        public async Task<IActionResult> GetAllReport(int PlanId, string keyword = "", int page = 1)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);

            var paging = new ForPaging(page);

            var result = await _service.GetAllUnifiedReportsPaged(UserId, PlanId, keyword, paging);

            if (result.Reports.Any())
                return Ok(result);
            else
                return NotFound(new { message = "目前無檢舉資料" });
        }



        [HttpGet("GetAllPostReport/{PlanId}")]
        public async Task<IActionResult> GetAllPostReport(int PlanId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetAllPostReport(UserId, PlanId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }
        [HttpGet("GetAllReplyReport/{PlanId}")]
        public async Task<IActionResult> GetAllReplyReport(int PlanId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetAllReplyReport(UserId, PlanId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }

        // GET api/<ReportController>/5
        [HttpGet("Reply/{R_Report_Id}")]
        public async Task<IActionResult> GetReplyReport(int R_Report_Id)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetReplyReport(UserId, R_Report_Id);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }

        // POST api/<ReportController>
        [HttpPost("{PlanId}/{PostId}")]
        public async Task<IActionResult> CreatePostReport(int PlanId, int PostId, [FromBody] Post_ReportDTO PRData)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.CreatePostReport(UserId, PlanId, PostId, PRData);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("DoingPostAction/{P_Report_Id}")]
        public async Task<IActionResult> DoPost_ReportAction(int P_Report_Id , [FromBody] Post_ReportDTO DoingAction)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.DoPost_ReportAction(UserId ,P_Report_Id, DoingAction);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }
        [HttpPost("{PlanId}/{PostId}/{ReplyId}")]
        public async Task<IActionResult> CreateReplyReport(int PlanId, int PostId, int ReplyId, [FromBody] Reply_ReportDTO RRData)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.CreateReplyReport(UserId, PlanId, PostId, ReplyId, RRData);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPut("DoingReplyAction/{R_Report_Id}")]
        public async Task<IActionResult> DoReply_ReportAction(int R_Report_Id, [FromBody] Reply_ReportDTO DoingAction)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.DoReply_ReportAction(UserId, R_Report_Id, DoingAction);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }
    }
}
