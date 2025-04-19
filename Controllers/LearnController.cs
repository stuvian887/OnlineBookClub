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
        public async Task<IActionResult> GetAllLearn()
        {
            return Ok(await _service.GetAllLearn()) ;
        }

        // GET api/<LearnController>/5
        [HttpGet("Index/{PlanId}")]
        public async Task<IActionResult> Get(int PlanId)
        {
            return Ok(await _service.GetLearn(PlanId));
        }

        // POST api/<LearnController>
        [HttpPost("Create/{PlanId}")]
        public async Task<IActionResult> CreateLearn(int PlanId, [FromBody] LearnDTO newData)
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
        [HttpPut("Update/{PlanId}/{Learn_Index}")]
        public async Task<IActionResult> UpdateLearn(int PlanId, int Learn_Index, [FromBody] LearnDTO updateData)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.UpdateLearn(UserId, PlanId, Learn_Index, updateData);
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
        [HttpDelete("Delete/{PlanId}/{Learn_Index}")]
        public async Task<IActionResult> Delete(int PlanId, int Learn_Index)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            var result = await _service.DeleteLearn(UserId, PlanId, Learn_Index);
            if (result.Item1 != null)
            {
                return Ok(result.Message);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpGet("Answer_Record/{PlanId}/{Learn_Index}")]
        public async Task<IActionResult> GetAnswer_Record(int PlanId, int Learn_Index)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            return Ok(await _service.GetAnswer_Record(UserId, PlanId, Learn_Index));
        }
        [HttpPost("Answer_Record")]
        public async Task<IActionResult> CreateAnswer_Record([FromBody] AnswerSubmissionDTO Answer)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            return Ok(await _service.CreateAnswer_Record(UserId, Answer));
        }
        [HttpPut("PassProgress/{PlanId}/{Learn_Index}")]
        public async Task<IActionResult> PassProgress(int PlanId , int Learn_Index)
        {
            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);
            await _service.PassProgressAsync(UserId, PlanId, Learn_Index);
            return Ok(new { message = "進度通過!" });
        }
    }
}


