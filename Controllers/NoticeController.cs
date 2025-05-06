using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.Models;
using OnlineBookClub.Services;  // 這裡記得根據你的專案實際的Service位置修改
using OnlineBookClub.Token;
using System.Threading.Tasks;

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoticeController : ControllerBase
    {
        private readonly NoticeService _noticeService;
        private readonly JwtService _jwtService;
        public NoticeController(NoticeService noticeService, JwtService jwtService)
        {
            _noticeService = noticeService;
            _jwtService = jwtService;
        }

        // 取得特定使用者的通知清單
        [HttpGet]
        public async Task<IActionResult> GetUserNotices()
        {
            var token = HttpContext.Request.Cookies["JWT"];
            int userid = Convert.ToInt32(_jwtService.GetUserIdFromToken(token));
            var notices = await _noticeService.GetNoticesByUserIdAsync(userid);
            return Ok(notices);
        }

        // 新增一則通知
        [HttpPost]
        public async Task<IActionResult> CreateNotice([FromBody] Notice notice)
        {
            await _noticeService.CreateNoticeAsync(notice);
            return Ok(new { message = "通知新增成功" });
        }

        // （如果未來要做已讀）標記某個通知為已讀
        [HttpPost("read/{notice_Id}")]
        public async Task<IActionResult> MarkNoticeAsRead(int notice_Id)
        {
            await _noticeService.MarkNoticeAsReadAsync(notice_Id);
            return Ok(new { message = "通知已標記為已讀" });
        }
        [HttpPost("check-upcoming")]
        public async Task<IActionResult> CheckUpcomingLearnings()
        {
            var token = HttpContext.Request.Cookies["JWT"];
            int userId = Convert.ToInt32(_jwtService.GetUserIdFromToken(token));

            await _noticeService.CheckAndNotifyUpcomingLearnings(userId);

            return Ok(new { message = "即將到期學習通知檢查完成" });
        }
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadNotices()
        {
            var token = HttpContext.Request.Cookies["JWT"];
            int userId = Convert.ToInt32(_jwtService.GetUserIdFromToken(token));
            var unread = await _noticeService.GetUnreadNoticesAsync(userId);
            return Ok(unread);
        }


    }
}
