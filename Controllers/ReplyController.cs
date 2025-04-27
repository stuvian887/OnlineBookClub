using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepliesController : ControllerBase
    {
        private readonly ReplyService _service;

        public RepliesController(ReplyService service)
        {
            _service = service;
        }
        [HttpGet("reply/{replyid}")]
        public async Task<IActionResult> GetreplyById(int replyid)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var post = await _service.GetReplyByIdAsync(email, replyid);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        

        [HttpPost]
        
        public async Task<IActionResult> CreateReply([FromForm] CreateReply dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var reply = await _service.CreateReplyAsync(userId, dto);
            return Ok(reply);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetReplies(int postId)
        {
            var replies = await _service.GetRepliesByPostIdAsync(postId, Request);
            return Ok(replies);
        }

        [HttpPut("{replyId}")]
        
        public async Task<IActionResult> UpdateReply(int replyId, [FromBody] CreateReply dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _service.UpdateReplyAsync(replyId, userId, dto);
            return result ? Ok() : Forbid();
        }

        [HttpDelete("{replyId}")]
        
        public async Task<IActionResult> DeleteReply(int replyId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _service.DeleteReplyAsync(replyId, userId);
            return result ? NoContent() : Forbid();
        }
    }
}
