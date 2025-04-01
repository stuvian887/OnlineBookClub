using System.ComponentModel.DataAnnotations;

namespace OnlineBookClub.DTO
{
    public class ChengePasswordDTO
    {
        [Required(ErrorMessage = " 請輸入密碼 ")]
        public string oldpassword { get; set; }
        [Required(ErrorMessage = " 請輸入密碼 ")]
        public string newpassword { get; set; }
        [Required(ErrorMessage = " 請輸入密碼 ")]
        [Compare("newpassword", ErrorMessage = " 兩次密碼輸入不一致 ")]
        public string Chenckpassword { get; set; }
    }
}
