﻿namespace OnlineBookClub.DTO
{
    public class MemberProgressDTO
    {
        public int User_Id { get; set; }
        public string UserName { get; set; }
        public string JoinDate { get; set; }
        public string ProgressPercent { get; set; }
        public string LearnName { get; set; }
        public bool IsComplete { get; set; }
    }
}
