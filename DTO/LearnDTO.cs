using OnlineBookClub.Models;

namespace OnlineBookClub.DTO
{
    public class LearnDTO
    {

        public int Plan_Id { get; set; }

        public string Learn_Name { get; set; } = null!;

        public int Learn_Index { get; set; }

        public int Pass_Standard { get; set; }
        public DateTime DueTime { get; set; }
        public bool Manual_Check { get; set; }
        public virtual ICollection<ProgressTracking> ProgressTracking {  get; set; }
    }
}
