using OnlineBookClub.Models;

namespace OnlineBookClub.DTO
{
    public class RegisterDTO
    {
        public Members newMember { get; set; }
        public string Password { get; set; }
        public string PasswordCheck { get; set; }
    }
}
