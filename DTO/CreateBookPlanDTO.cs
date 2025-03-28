using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class CreateBookPlanDTO
    {
        [Required]
        public string PlanName { get; set; }
        [Required]
        public string PlanGoal { get; set; }
        [Required]
        public string PlanType { get; set; }
        [Required]
        public string PlanSubject { get; set; }
        public bool IsPublic { get; set; }
    }
}
