namespace OnlineBookClub.DTO
{
    public class ReplyDTO
    {
        public int ReplyId { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string Content { get; set; }
        public string? ImgPath { get; set; }
        public DateTime CreateTime { get; set; }

        public string Name { get; set; }
        public string MemberPath { get; set; }
    }
}
