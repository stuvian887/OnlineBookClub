using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using OnlineBookClub.Token;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookPlanController : ControllerBase
    {
        private readonly BookPlanService _service;
        private readonly JwtService _jwtService;
        public BookPlanController(BookPlanService service, JwtService jwtService)
        {
            _service = service;
            _jwtService = jwtService;
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetAllPublicPlans()
        {
            var plans = await _service.GetAllPublicPlans();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _service.GetById(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookPlanDTO bookPlanDto)
        {
            var token = HttpContext.Request.Cookies["JWT"];
          

            var userId = _jwtService.GetUserIdFromToken(token);

            int id = Convert.ToInt32(userId);
            var plan = await _service.Create(bookPlanDto,id);
            return CreatedAtAction(nameof(GetById), new { id = plan.Plan_Id }, plan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookPlanDTO bookPlanDto)
        {
            var updatedPlan = await _service.Update(id, bookPlanDto);
            if (updatedPlan == null) return NotFound();
            return Ok(updatedPlan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
