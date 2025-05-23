﻿using Microsoft.EntityFrameworkCore;
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
            reply.ReplyTime = DateTime.Now;
            _context.Reply.Add(reply);
            await _context.SaveChangesAsync();
            return reply;
        }

        public async Task<IEnumerable<Reply>> GetRepliesByPostIdAsync(int postId)
        {
            return await _context.Reply
                .Where(r => r.Post_Id == postId && !r.IsDeleted)
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
            reply.IsDeleted = true;
            _context.Reply.Update(reply);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Reply?> GeReplyByIdWithUserAsync(int ReplyID)
        {
            return await _context.Reply
                .Include(p => p.User )
                .FirstOrDefaultAsync(p => p.Reply_Id == ReplyID);
        }
        public async Task<IEnumerable<Reply>> GetReplyByUserIdAsync(int userId)
        {
            return await _context.Reply
                .Where(p => p.User_Id == userId&& !p.IsDeleted)
                .OrderByDescending(p => p.ReplyTime)
                .ToListAsync();
        }
    }
}
