using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class BookPlanDTO
    {
        public string PlanName { get; set; }
        public string PlanGoal { get; set; }
        public string PlanType { get; set; }
        public string PlanSubject { get; set; }
        public bool IsPublic { get; set; }
        public bool IsComplete { get; set; }
        public int UserId { get; set; }
    }
}
