namespace OnlineBookClub.DTO
{
    public class ReplyDTO
    {
        public int PostId { get; set; }
        public string ReplyContent { get; set; }
        public string? ReplyImg { get; set; }
        public IFormFile? ReplyCover { get; set; }
    }
}
