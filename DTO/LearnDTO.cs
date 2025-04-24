using OnlineBookClub.Models;
using System.Data;
using System.Text.Json.Serialization;

namespace OnlineBookClub.DTO
{
    public class LearnDTO
    {

        public int Plan_Id { get; set; }

        public string Learn_Name { get; set; } = null!;

        public int Learn_Index { get; set; }

        public int Pass_Standard { get; set; }
        public DateTime DueTime { get; set; }
        public string RecentlyLearnDate { get; set; }
        public string RecentlyLearn { get; set; } = null;
        public bool Manual_Check { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProgressTracking> ProgressTracking { get; set; }
    }
}
