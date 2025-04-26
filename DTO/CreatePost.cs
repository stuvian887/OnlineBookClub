namespace OnlineBookClub.DTO
{
    public class CreatePost
    {
        public int UserId { get; set; }
        public int PlanId { get; set; }
        public string Content { get; set; }
        public IFormFile? PostCover { get; set; }
    }
}
