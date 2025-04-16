using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
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
        public LearnController(LearnService service)
        {
            _service = service;
        }
        // GET: api/<LearnController>
        [HttpGet("Index")]
        public async Task<IEnumerable<LearnDTO>> GetAllLearn()
        {
            return await _service.GetAllLearn();
        }

        // GET api/<LearnController>/5
        [HttpGet("Index/{PlanId}")]
        public async Task<IEnumerable<LearnDTO>> Get(int PlanId)
        {
            return await _service.GetLearn(PlanId);
        }

        // POST api/<LearnController>
        [HttpPost("Create/{PlanId}")]
        public async Task<IActionResult> Post(int PlanId, [FromBody] LearnDTO newData)
        {
            LearnDTO result = await _service.CreateLearn(PlanId, newData);
            if (result != null)
            {
                return Ok(new { message = "學習內容新增成功" });
            }
            else
            {
                return BadRequest(new { Message = "發生錯誤" });
            }
        }

        // PUT api/<LearnController>/5
        [HttpPut("Update/{PlanId}/{LearnId}")]
        public async Task<IActionResult> Put(int PlanId, int LearnId, [FromBody] LearnDTO updateData)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.UpdateLearn(UserId, PlanId, LearnId, updateData);
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
        [HttpDelete("Delete/{PlanId}/{LearnId}")]
        public async Task<IActionResult> Delete(int PlanId, int LearnId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.DeleteLearn(UserId, PlanId, LearnId);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet("Answer_Record/{PlanId}/{LearnId}")]
        public async Task<IEnumerable<Answer_RecordDTO>> GetAnswer_Record(int PlanId, int LearnId)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            return await _service.GetAnswer_Record(UserId, PlanId, LearnId);
        }
        [HttpPost("Answer_Record")]
        public async Task<IEnumerable<Answer_RecordDTO>> CreateAnswer_Record(AnswerSubmissionDTO Answer)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            return await _service.CreateAnswer_Record(UserId, Answer);
        }
    }
}


