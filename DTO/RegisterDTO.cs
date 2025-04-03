using OnlineBookClub.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "Email 是必填欄位")]
        [EmailAddress(ErrorMessage = "Email 格式不正確")]
        public string Email { get; set; }
        [Required(ErrorMessage = "使用者名稱是必填欄位")]
        [StringLength(50, ErrorMessage = "使用者名稱長度最多 50 個字元")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "密碼是必填欄位")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密碼長度需介於 6 到 20 個字元")]
        public string Password { get; set; }
        [Required(ErrorMessage = "請再次輸入密碼")]
        [Compare("Password", ErrorMessage = "兩次輸入的密碼不相同")]

        public string PasswordCheck { get; set; }

    }
}
