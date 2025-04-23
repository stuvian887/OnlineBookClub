namespace OnlineBookClub.DTO
{
    public class PostDTO
    {
        public int PlanId { get; set; }
        public string Content { get; set; }
        public string? ImgPath { get; set; }
        public IFormFile? PostCover { get; set; }
    }
}
