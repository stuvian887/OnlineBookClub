using OnlineBookClub.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class RegisterDTO
    {
        [Required]
       public string email { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordCheck { get; set; }

    }
}
