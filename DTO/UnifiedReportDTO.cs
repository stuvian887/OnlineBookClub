namespace OnlineBookClub.DTO
{
    public class UnifiedReportDTO
    {
        public int ReportId { get; set; }           // 共用欄位，對應 Post 或 Reply 的 ReportId
        public int TargetId { get; set; }           // 對應 PostId 或 ReplyId
        public string Type { get; set; }            // "Post" 或 "Reply"
        public string Action { get; set; }          // 管理操作狀態
        public string ReportText { get; set; }      // 檢舉原因
        public DateTime ReportTime { get; set; }    // 檢舉時間
    }

}
