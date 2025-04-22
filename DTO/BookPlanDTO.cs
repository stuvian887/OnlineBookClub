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
        public bool IsComplete { get; set; }
        public string CreatorName { get; set; }

    }
}
