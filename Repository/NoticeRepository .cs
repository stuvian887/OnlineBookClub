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
        public async Task CheckAndNotifyUpcomingLearnings(int userId)
        {
            var progresses = await _context.ProgressTracking
                .Where(p => p.User_Id == userId)
                .Select(p => p.Learn)
                .Where(l => l.DueTime > DateTime.Now && l.DueTime <= DateTime.Now.AddDays(3))
                .ToListAsync();

            foreach (var learn in progresses)
            {
                string message = $"提醒：學習項目「{learn.Learn_Name}」將於 {learn.DueTime:yyyy-MM-dd} 到期";

                // 檢查通知是否已存在（用 User_Id + Message 判斷）
                bool alreadyReminded = await _context.Notice
                    .AnyAsync(n => n.User_Id == userId && n.Message == message);

                if (!alreadyReminded)
                {
                    var notice = new Notice
                    {
                        User_Id = userId,
                        NoticeTime = DateTime.Now,
                        Message = message
                    };
                    _context.Notice.Add(notice);
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
