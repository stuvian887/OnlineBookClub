using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Service;
using System.Numerics;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlanMemberController : ControllerBase
    {
        private readonly PlanMemberService _planMembersService;

        public PlanMemberController(PlanMemberService planMembersService)
        {
            _planMembersService = planMembersService;
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinPlan([FromForm] int planid)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _planMembersService.JoinPlanAsync(UserId , planid);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("leave")]
        public async Task<IActionResult> LeavePlan([FromForm] int planid)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _planMembersService.LeavePlanAsync(UserId , planid);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpGet("isMember")]
        public async Task<IActionResult> IsMember(int planId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var isMember = await _planMembersService.IsUserInPlanAsync(UserId, planId);
            return Ok(new { IsMember = isMember });
        }

        [HttpPost("copyPlan")]
        public async Task<IActionResult> CopyPlan([FromForm] int planid)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _planMembersService.CopyPlanAsync(planid, UserId);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
