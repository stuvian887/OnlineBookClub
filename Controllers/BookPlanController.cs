using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookPlanController : ControllerBase
    {
        private readonly BookPlanService _service;

        public BookPlanController(BookPlanService service)
        {
            _service = service;
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
            var plan = await _service.Create(bookPlanDto);
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
