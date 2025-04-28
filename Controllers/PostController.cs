using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using OnlineBookClub.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly PostService _service;
     
        public PostsController(PostService service)
        {
            _service = service;
        }
        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                return Unauthorized();
            }

            var post = await _service.GetPostByIdAsync( postId, Request);
            if (post == null)
                return NotFound();
            return Ok(post);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyPosts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = int.Parse(userIdClaim.Value);
            var myPosts = await _service.GetPostsByUserIdAsync(userId,email,Request);
            return Ok(myPosts);
        }


        [HttpPost]
        
        public async Task<IActionResult> CreatePost([FromForm] CreatePost dto )
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var userName = User.FindFirst("FullName")?.Value
             ?? User.FindFirst(ClaimTypes.Name)?.Value
             ?? "Unknown";
            var result = await _service.CreatePostAsync(email, userId, userName, dto);

            return Ok(result);
        }

        [HttpGet("{planId}")]
        public async Task<IActionResult> GetPosts(int planId, [FromQuery] string? keyword)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var posts = await _service.GetPostsByPlanIdAsync(email, planId, keyword, Request);  // 傳遞 keyword
            return Ok(posts);
        }


        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromForm] CreatePost dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _service.UpdatePostAsync(postId, userId, dto);
            return result ? Ok() : Forbid();
        }

        [HttpDelete("{postId}")]
        
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _service.DeletePostAsync(postId, userId);
            return result ? NoContent() : Forbid();
        }
    }


}
