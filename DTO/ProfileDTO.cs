using System.Text.Json.Serialization;

namespace OnlineBookClub.DTO
{
    public class ProfileDTO
    {
        public string Name { get; set; }

        //public string email { get; set; }
        public DateTime? Birthday { get; set; }  
        public bool Gender { get; set; }
        public string ProfilePictureUrl { get; set; }
    }
}
