namespace OnlineBookClub.DTO
{
    public class BookDTO
    {
    
        public int platid { get; set; }
        public string BookName { get; set; } = "null";

        public string Description { get; set; } = "null";

        public string Link { get; set; } = "null";
        public string bookurl { get; set; } = "null";
        public IFormFile? BookCover { get; set; } 
    }
}
