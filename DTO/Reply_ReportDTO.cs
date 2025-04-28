namespace OnlineBookClub.DTO
{
    public class Reply_ReportDTO
    {
        public int R_Report_Id { get; set; }

        public int Reply_Id { get; set; }

        public string Action { get; set; }

        public string Report_text { get; set; }
        public DateTime Report_Time { get; set; }
    }
}
