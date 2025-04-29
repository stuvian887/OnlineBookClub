using OnlineBookClub.Models;

namespace OnlineBookClub.DTO
{
    public class ReportPageResultDTO
    {
        public string Keyword { get; set; } = ""; // 搜尋用的關鍵字（例如檢舉內容）

        public List<UnifiedReportDTO> Reports { get; set; } = [];

        public ForPaging Paging { get; set; } = new ForPaging(); // 分頁資料
    }
}
