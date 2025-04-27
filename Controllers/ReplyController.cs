using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Service;
using OnlineBookClub.Services;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepliesController : ControllerBase
    {
        private readonly ReplyService _service;
        private readonly PostService _postService;
        private readonly MembersService _membersService;
        private readonly NoticeService _noticeService;

        public RepliesController(ReplyService service , PostService postService, MembersService membersService, NoticeService noticeService)
        {
            _service = service;
            _postService = postService;
            _membersService = membersService;
            _noticeService = noticeService;
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
            
            var post = await _postService.GetPostByIdAsync(dto.PostId);
            var user = await _membersService.GetUserById(post.UserId);
            var reply = await _service.CreateReplyAsync(userId, dto);
            var notification = new Notice
            {

                User_Id = post.UserId,  // 通知給原貼文作者
                NoticeTime = DateTime.Now,
                Message = $"{user.UserName} 回覆了您的貼文：{reply.ReplyContent}",
                User = user,
            };
            await _noticeService.CreateNoticeAsync(notification);  // 保存通知到資料庫
            await _noticeService.GetNoticesByUserIdAsync(post.UserId);

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
