﻿namespace OnlineBookClub.DTO
{
    public class CreateReply
    {
        public int UserId { get; set; }
        public int PostId { get; set; }
        public string ReplyContent { get; set; }
        public IFormFile? ReplyCover { get; set; }
    }
}
