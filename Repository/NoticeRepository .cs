using Microsoft.EntityFrameworkCore;
using OnlineBookClub.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBookClub.Repositories
{
    public class NoticeRepository 
    {
        private readonly OnlineBookClubContext _context;

        public NoticeRepository(OnlineBookClubContext context)
        {
            _context = context;
        }

        public async Task<List<Notice>> GetNoticesByUserIdAsync(int userId)
        {
            return await _context.Notice
                .Where(n => n.User_Id == userId)
                .OrderByDescending(n => n.NoticeTime)
                .ToListAsync();
        }

        public async Task CreateNoticeAsync(Notice notice)
        {
            _context.Notice.Add(notice);
            await _context.SaveChangesAsync();
        }

        public async Task MarkNoticeAsReadAsync(int noticeId)
        {
            var notice = await _context.Notice.FindAsync(noticeId);
            if (notice != null)
            {
                // 這裡需要你的 Notice 有 IsRead 欄位才可以標記
                // notice.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
