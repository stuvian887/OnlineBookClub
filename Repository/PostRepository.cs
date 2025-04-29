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
                .FirstOrDefaultAsync(p => p.Post_Id == postId && !p.IsDeleted);
        }
        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(int userId)
        {
            return await _context.Post
                .Where(p => p.User_Id == userId && !p.IsDeleted)
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

        public async Task<IEnumerable<Post>> GetPostsByPlanIdAsync(int planId, string? keyword)
        {
            var query = _context.Post
                .Where(p => p.Plan_Id == planId && !p.IsDeleted);

            // 如果有提供 keyword，根據 keyword 過濾內容
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Content.Contains(keyword));  // 搜尋帖子內容
            }

            return await query.OrderByDescending(p => p.CreateTime).ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            return await _context.Post.FirstOrDefaultAsync(p => p.Post_Id == postId && !p.IsDeleted);
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
            post.IsDeleted = true;
            _context.Post.Update(post);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
