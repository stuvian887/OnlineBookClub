namespace OnlineBookClub.DTO
{
    public class BookDTO
    {

        public string BookName { get; set; }

        public string Description { get; set; }

        public string Link { get; set; }
        public string bookurl { get; set; }
        public IFormFile? BookCover { get; set; }
    }
}
