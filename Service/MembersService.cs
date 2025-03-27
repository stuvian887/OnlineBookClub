using OnlineBookClub.Models;
using System.Security.Cryptography;
using System.Text;
using OnlineBookClub.Repository;
using OnlineBookClub.DTO;
namespace OnlineBookClub.Service
{
    public class MembersService
    {
        private readonly IConfiguration _config;
        private readonly OnlineBookClubContext _OnlineBookClubContext;
        private readonly MembersRepository _MembersRepository;
        public MembersService(OnlineBookClubContext OnlineBookClubContext, IConfiguration config, MembersRepository MembersRepository)
        {
            _OnlineBookClubContext = OnlineBookClubContext;

            _config = config;
            _MembersRepository= MembersRepository;
        }
        public void Register(Members NewMember)
        {
           
           _MembersRepository.Add(NewMember);
            
        }
        public string HashPassword(string Password)
        {
            string saltkey = "1q2w3e4r5t6y7u8ui9o0po7tyy";
            string saltAndPassword = String.Concat(Password, saltkey);
            SHA256 sha256Hasher = SHA256.Create();
            byte[] PassswordData = Encoding.Default.GetBytes(saltAndPassword);
            byte[] HashDate = sha256Hasher.ComputeHash(PassswordData);
            string Hashreseult = Convert.ToBase64String(HashDate);
            return Hashreseult;
        }
        private Members GetDataEmail(string Email)
        {
            Members Data = new Members();
            var sql = (from a in _OnlineBookClubContext.Members where a.Email == Email select a).SingleOrDefault();
            return sql;
        }
        public bool EmailCheck(string Email)
        {
            Members Data = GetDataEmail(Email);
            return (Data == null);
        }
        public string EmailValidate(string Email, string AuthCode)
        {
            Members ValidateMember = GetDataEmail(Email);
            string Validatestr = string.Empty;
            if (ValidateMember != null && ValidateMember.AuthCode == AuthCode)
            {
                var update = (from a in _OnlineBookClubContext.Members where a.Email == ValidateMember.Email select a).SingleOrDefault();
                update.AuthCode = string.Empty;  // 清空驗證碼，標記為已驗證
                _OnlineBookClubContext.SaveChanges(); // 儲存變更
                return "帳號信箱驗證成功，現在可以登入了";
            }
            else
            {
                Validatestr = "驗證碼錯誤，請重新確認或再註冊";
            }
            return Validatestr;
        }
    }
}
