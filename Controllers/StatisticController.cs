using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : ControllerBase
    {
        private readonly StatisticService _service;

        public StatisticController(StatisticService service)
        {
            _service = service;
        }

        [HttpGet("{planId}")]
        public async Task<IActionResult> GetByPlanId(int planId)
        {
            var stat = await _service.GetStatisticByPlanIdAsync(planId);
            if (stat == null)
                return NotFound();
            return Ok(stat);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStatistics()
        {
            var stats = await _service.GetAllStatisticsAsync();
            return Ok(stats);
        }

    }

}
