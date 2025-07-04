﻿using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using OnlineBookClub.Token;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookPlanController : ControllerBase
    {
        private readonly BookPlanService _service;
        private readonly JwtService _jwtService;
        [ActivatorUtilitiesConstructor]
        private readonly StatisticService _statisticService;
        private readonly BookService _bookService;
        private readonly LearnService _learnService;
        private readonly ChapterService _chapterService;
        public BookPlanController(BookPlanService service, JwtService jwtService, StatisticService statisticService,BookService bookService , LearnService learnService, ChapterService chapterService)
        {
            _service = service;
            _jwtService = jwtService;
            _statisticService = statisticService;
            _bookService = bookService;
            _learnService = learnService;
            _chapterService = chapterService;
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetAllPublicPlans([FromQuery]string? keyword, [FromQuery] int page, [FromQuery] string order, [FromQuery] string category)
        {
            var token = HttpContext.Request.Cookies["JWT"];
            int userid = Convert.ToInt32(_jwtService.GetUserIdFromToken(token));
            
            var result = await _service.GetPublicPlansAsync(userid,keyword, page,order, category);

            return Ok(result); 
        }
        




        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var plan = await _service.GetById(id);
            if (plan == null) return NotFound();
            await _statisticService.AddViewTimesAsync(id);
            return Ok(plan);
        }

        [HttpGet("youbookplan")]

        public async Task<IActionResult> GetByUserId([FromQuery] string? keyword, [FromQuery] int page )
        {

            var UserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int UserId = int.Parse(UserIdClaim.Value);

            var plan = await _service.GetuserById(UserId, keyword, page);

            if (plan == null) return NotFound();
            return Ok(plan);
        }
        [HttpGet("getmembersbookplan/{user_Id}")]
        public async Task<IActionResult> GetMembersBookPlan(int user_Id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int leaderId = int.Parse(userIdClaim.Value);

            var memberPlans = await _service.GetMembersBookPlans(leaderId, user_Id);
            return Ok(new { Memberbookplan = memberPlans });
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookPlanDTO bookPlanDto)
        {
            var token = HttpContext.Request.Cookies["JWT"];
          

            var userId = _jwtService.GetUserIdFromToken(token);

            int id = Convert.ToInt32(userId);
            var plan = await _service.Create(bookPlanDto,id);
            int planid = plan.Plan_Id;
            await _statisticService.CreateStatistic(planid);
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
        [HttpPost("copy/{planId}")]
        public async Task<IActionResult> CopyPlan(int planId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var result = await _service.CopyPlanAsync(planId, userId);
            if (result==null)
                return NotFound("找不到");
            var book = await _bookService.GetBookByPlanIdAsync(planId, Request);
            await _bookService.AddBookAsync(result.Plan_Id, book);

            

            //取的原本的章節
            var OriginalChapter = await _chapterService.GetChapter(planId);
            
            //跑五次(假設五個)
            foreach(var chapter in OriginalChapter)
            {
                //複製後，回傳新的章節
                var ChapterResult = await _chapterService.CopyChapters(planId, result.Plan_Id ,chapter.Chapter_Id);
                var newChapter = await _chapterService.GetSingleChapter(ChapterResult.Chapter_Id);
                var learn = await _learnService.GetLearn(userId, planId , chapter.Chapter_Id);
                //跑五次(假設五個)
                foreach (var item in learn)
                {
                    await _learnService.copy(userId, result.Plan_Id, newChapter.Chapter_Id ,item );
                }
                await _statisticService.AddCopyCountAsync(planId);
            }
            return Ok("Plan copied successfully.");
        }

    }
}
