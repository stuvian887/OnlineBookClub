using OnlineBookClub.Models;

namespace OnlineBookClub.DTO
{
    public class ProgressTrackingDTO
    {
        public int Progress_Id { get; set; }

        public int User_Id { get; set; }

        public int Learn_Id { get; set; }
        public int Learn_Index { get; set; }

        public bool Status { get; set; }

        public DateTime? CompletionDate { get; set; }

    }
}
