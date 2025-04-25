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

        [HttpPost]
        
        public async Task<IActionResult> CreatePost([FromForm] PostDTO dto )
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            var userName = User.FindFirst("FullName")?.Value
             ?? User.FindFirst(ClaimTypes.Name)?.Value
             ?? "Unknown";
            var result = await _service.CreatePostAsync(userId, userName, dto);

            return Ok(result);
        }

        [HttpGet("{planId}")]
        public async Task<IActionResult> GetPosts(int planId, HttpRequest request)
        {
            var posts = await _service.GetPostsByPlanIdAsync(planId,request);
            
            return Ok(posts);
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromForm] PostDTO dto)
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
