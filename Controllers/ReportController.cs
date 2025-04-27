using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using System.Security.Claims;

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
        [HttpGet("{PlanId}/{PostId}")]
        public async Task<IActionResult> GetPostReport(int PlanId, int PostId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetPostReport(UserId, PlanId, PostId);
            if(result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new {message ="發生錯誤"});
            }
        }
        [HttpGet("GetAllPostReport")]
        public async Task<IActionResult> GetAllPostReport()
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetAllPostReport(UserId);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }
        [HttpGet("GetAllReplyReport")]
        public async Task<IActionResult> GetAllReplyReport()
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetAllReplyReport(UserId);
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
        [HttpGet("{PlanId}/{PostId}/{ReplyId}")]
        public async Task<IActionResult> GetReplyReport(int PlanId, int PostId, int ReplyId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.GetReplyReport(UserId, PlanId, PostId, ReplyId);
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
    }
}
