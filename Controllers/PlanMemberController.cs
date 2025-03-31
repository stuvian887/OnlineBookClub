using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Service;

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
        public async Task<IActionResult> JoinPlan([FromBody] PlanMembers joinPlanDto)
        {
            var result = await _planMembersService.JoinPlanAsync(joinPlanDto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpDelete("leave")]
        public async Task<IActionResult> LeavePlan([FromBody] PlanMembers leavePlanDto)
        {
            var result = await _planMembersService.LeavePlanAsync(leavePlanDto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }

        [HttpGet("isMember")]
        public async Task<IActionResult> IsMember(int userId, int planId)
        {
            var isMember = await _planMembersService.IsUserInPlanAsync(userId, planId);
            return Ok(new { IsMember = isMember });
        }

        [HttpPost("copyPlan")]
        public async Task<IActionResult> CopyPlan([FromBody] PlanMembers copyPlanDto)
        {
            var result = await _planMembersService.CopyPlanAsync(copyPlanDto);
            return result.Success ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}
