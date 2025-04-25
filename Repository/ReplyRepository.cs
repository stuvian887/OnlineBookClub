using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;

namespace OnlineBookClub.Repository
{
    public class ReplyRepository 
    {
        private readonly OnlineBookClubContext _context;

        public ReplyRepository(OnlineBookClubContext context)
        {
            _context = context;
        }

        public async Task<Reply> CreateReplyAsync(Reply reply)
        {
            
            _context.Reply.Add(reply);
            await _context.SaveChangesAsync();
            return reply;
        }

        public async Task<IEnumerable<Reply>> GetRepliesByPostIdAsync(int postId)
        {
            return await _context.Reply
                .Where(r => r.Post_Id == postId)
                .OrderByDescending(r => r.ReplyTime)
                .ToListAsync();
        }

        public async Task<Reply?> GetReplyByIdAsync(int replyId)
        {
            return await _context.Reply.FindAsync(replyId);
        }

        public async Task<bool> UpdateReplyAsync(Reply reply)
        {
            _context.Reply.Update(reply);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteReplyAsync(int replyId)
        {
            var reply = await GetReplyByIdAsync(replyId);
            if (reply == null) return false;
            _context.Reply.Remove(reply);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
