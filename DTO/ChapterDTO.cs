namespace OnlineBookClub.DTO
{
    public class ChapterDTO
    {
        public int Chapter_Id { get; set; }
        public int Chapter_Index { get; set; }
        public string Chapter_Name { get; set; }
        public int Plan_Id { get; set; }
        public bool Status { get; set; }
        public double PersentOfMemberPass { get; set; }
    }
}
