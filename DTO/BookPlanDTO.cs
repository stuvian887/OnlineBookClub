using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class BookPlanDTO
    {
        public int Plan_ID { get; set; }
        public string Plan_Name { get; set; }
        public string Plan_Goal { get; set; }
        public string Plan_Type { get; set; }
        public string Plan_Suject { get; set; }
        public bool IsPublic { get; set; }
        public string RecentlyLearnDate { get; set; }
        public string RecentlyLearn { get; set; }
        public bool IsComplete { get; set; }
        public string CreatorName { get; set; }

        public string JoinDate { get; set; }
        public string ProgressPercent { get; set; }

        public int times { get; set; }

    }
}
