using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email 是必填欄位")]
        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
