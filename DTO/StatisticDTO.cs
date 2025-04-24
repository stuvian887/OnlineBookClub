namespace OnlineBookClub.DTO
{
    public class StatisticDTO
    {
        public int PlanId { get; set; }
        public int CopyCount { get; set; } = 0;
        public int UserCount { get; set; } = 0;
        public int ViewTimes { get; set; } = 0;
    }

}
