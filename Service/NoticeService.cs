using OnlineBookClub.Models;
using OnlineBookClub.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineBookClub.Services
{
    public class NoticeService 
    {
        private readonly NoticeRepository _noticeRepository;

        public NoticeService(NoticeRepository noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }

        public async Task<List<Notice>> GetNoticesByUserIdAsync(int userId)
        {
            return await _noticeRepository.GetNoticesByUserIdAsync(userId);
        }

        public async Task CreateNoticeAsync(Notice notice)
        {
            await _noticeRepository.CreateNoticeAsync(notice);
        }

        public async Task MarkNoticeAsReadAsync(int noticeId)
        {
            await _noticeRepository.MarkNoticeAsReadAsync(noticeId);
        }
        public async Task CheckAndNotifyUpcomingLearnings(int userId)
        {
            await _noticeRepository.CheckAndNotifyUpcomingLearnings(userId);
        }

    }
}
