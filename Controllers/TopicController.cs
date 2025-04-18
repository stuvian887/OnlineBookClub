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
        [HttpGet("{PlanId}/{Learn_Index}")]
        public async Task<IActionResult> GetTopic(int PlanId, int Learn_Index)
        {
            return Ok(await _service.GetTopicAsync(PlanId, Learn_Index));
        }

        // POST api/<TopicController>
        [HttpPost("{PlanId}/{Learn_Index}")]
        public async Task<IActionResult> Create(int PlanId, int Learn_Index, [FromBody] TopicDTO newData)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.CreateTopic(UserId , PlanId, Learn_Index, newData);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        // PUT api/<TopicController>/5
        [HttpPut("{PlanId}/{Learn_Index}/{QuestionId}")]
        public async Task<IActionResult> Update(int PlanId, int Learn_Index, int QuestionId, [FromBody] TopicDTO updateData)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.UpdateTopic(UserId , PlanId, Learn_Index, QuestionId, updateData);
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
        [HttpDelete("{PlanId}/{Learn_Index}/{QuestionId}")]
        public async Task<IActionResult> Delete(int PlanId , int Learn_Index, int QuestionId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.DeleteTopic(UserId , PlanId, Learn_Index, QuestionId);
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
