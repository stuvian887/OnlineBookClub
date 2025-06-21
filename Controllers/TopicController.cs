using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly TopicService _service;
        public TopicController(TopicService service)
        {
            _service = service;
        }
        // GET: api/<TopicController>
        [HttpGet("{PlanId}/{Chapter_Id}/{Learn_Index}")]
        public async Task<IActionResult> GetTopic(int PlanId,int Chapter_Id, int Learn_Index)
        {
            var result = await _service.GetTopicAsync(PlanId,Chapter_Id, Learn_Index);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(new { message = "發生錯誤" });
            }
        }

        // POST api/<TopicController>
        [HttpPost("{PlanId}/{Chapter_Id}/{Learn_Index}")]
        public async Task<IActionResult> Create(int PlanId, int Chapter_Id, int Learn_Index, [FromBody] TopicDTO newData)
        {
            int UserId = GetUser();
            var result = await _service.CreateTopic(UserId , PlanId, Chapter_Id, Learn_Index, newData);
            if (result.Item1 != null)
            {
                return Ok(new { result.Message });
            }
            else
            {
                return BadRequest(new { result.Message });
            }
        }

        // PUT api/<TopicController>/5
        [HttpPut("{PlanId}/{Chapter_Id}/{Learn_Index}/{QuestionId}")]
        public async Task<IActionResult> Update(int PlanId, int Chapter_Id, int Learn_Index, int QuestionId, [FromBody] TopicDTO updateData)
        {
            int UserId = GetUser();
            var result = await _service.UpdateTopic(UserId , PlanId, Chapter_Id, Learn_Index, QuestionId, updateData);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // DELETE api/<TopicController>/5
        [HttpDelete("{PlanId}/{Chapter_Id}/{Learn_Index}/{QuestionId}")]
        public async Task<IActionResult> Delete(int PlanId, int Chapter_Id, int Learn_Index, int QuestionId)
        {
            int UserId = GetUser();
            var result = await _service.DeleteTopic(UserId , PlanId, Chapter_Id, Learn_Index, QuestionId);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        private int GetUser()
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("發生異常錯誤，找不到登入的人是誰。");
            return int.Parse(UserIdClaim.Value);
        }
    }
}
