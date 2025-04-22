using OnlineBookClub.Models;

namespace OnlineBookClub.DTO
{
    public class BookPlanPageResultDTO
    {
        public string Keyword { get; set; } // 搜尋關鍵字

        public List<BookPlanDTO> Plans { get; set; } = [];

        public ForPaging Paging { get; set; }  // ⭐ 加入這個分頁資訊
    }
}
