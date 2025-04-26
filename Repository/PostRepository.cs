using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class PostRepository
    {
        private readonly OnlineBookClubContext _context;

        public PostRepository(OnlineBookClubContext context)
        {
            _context = context;
        }
        public async Task<Post?> GetPostByIdWithUserAsync(int postId)
        {
            return await _context.Post
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Post_Id == postId);
        }
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId)
        {
            return await _context.Post
                .Where(p => p.User_Id == userId)
                .OrderByDescending(p => p.CreateTime)
                .ToListAsync();
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            post.CreateTime = DateTime.UtcNow;
            _context.Post.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<IEnumerable<Post>> GetPostsByPlanIdAsync(int planId)
        {
            return await _context.Post
                .Where(p => p.Plan_Id == planId)
                .OrderByDescending(p => p.CreateTime)
                .ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _context.Post.FindAsync(postId);
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            _context.Post.Update(post);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePostAsync(int postId)
        {
            var post = await _context.Post.FindAsync(postId);
            if (post == null) return false;
            _context.Post.Remove(post);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
