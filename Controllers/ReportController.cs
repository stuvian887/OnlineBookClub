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
            return Ok(await _service.GetPostReport(UserId, PlanId, PostId));
        }

        // GET api/<ReportController>/5
        [HttpGet("{PlanId}/{PostId}/{ReplyId}")]
        public async Task<IActionResult> GetReplyReport(int PlanId, int PostId, int ReplyId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            return Ok(await _service.GetReplyReport(UserId, PlanId, PostId, ReplyId));
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
