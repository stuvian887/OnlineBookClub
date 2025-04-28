namespace OnlineBookClub.DTO
{
    public class MemberProgressDTO
    {
        public int User_Id { get; set; }
        public string UserName { get; set; }
        public DateTime JoinDate { get; set; }
        public string ProgressPercent { get; set; }
    }
}
