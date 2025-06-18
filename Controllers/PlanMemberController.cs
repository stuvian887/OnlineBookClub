using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly StatisticService _statisticService;
        public PlanMemberController(PlanMemberService planMembersService,StatisticService statisticService)
        {
            _planMembersService = planMembersService;
            _statisticService = statisticService;
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
            await _statisticService.DecreaseUserCountAsync(planid);
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
        [HttpGet("getmembers/{planId}")]
        public async Task<IActionResult> getmembers(int planId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var Memberdata = await _planMembersService.Getmembers(planId,UserId);
            return Ok(new { Memberdata });
        }
        [HttpGet("isleader")]
        public async Task<IActionResult> isleader(int planId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var isLeader = await _planMembersService.isleader(planId, UserId);
            return Ok(new { isLeader });
        }
         
    }
}
