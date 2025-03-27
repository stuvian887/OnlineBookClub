using OnlineBookClub.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class RegisterDTO
    {
        
       
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordCheck { get; set; }
        public Members newMember { get; set; }
    }
}
