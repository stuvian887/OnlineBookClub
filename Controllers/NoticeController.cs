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
        [HttpPost("read/{noticeId}")]
        public async Task<IActionResult> MarkNoticeAsRead(int noticeId)
        {
            await _noticeService.MarkNoticeAsReadAsync(noticeId);
            return Ok(new { message = "通知已標記為已讀" });
        }
    }
}
