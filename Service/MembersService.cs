using OnlineBookClub.Models;
using System.Security.Cryptography;
using System.Text;
using OnlineBookClub.Repository;
using OnlineBookClub.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        public Members GetDataEmail(string Email)
        {
            
            Members Data = (from a in _OnlineBookClubContext.Members where a.Email == Email select a).SingleOrDefault();
            return Data;
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
        public void authcode(string email,string authcode)
        {
            var members = GetDataEmail(email);
            members.AuthCode = authcode;
            _MembersRepository.Update(members);
        }
        public string forgetpsEmailValidate(string Email, string AuthCode)
        {
            Members ValidateMember = GetDataEmail(Email);
            string Validatestr = string.Empty;
            if (ValidateMember != null && ValidateMember.AuthCode == AuthCode)
            {
                var update = (from a in _OnlineBookClubContext.Members where a.Email == ValidateMember.Email select a).SingleOrDefault();
                update.AuthCode = string.Empty;  // 清空驗證碼，標記為已驗證
                _OnlineBookClubContext.SaveChanges(); // 儲存變更
                return "帳號信箱驗證成功，請點擊下方連結前往更改密碼";
            }
            else
            {
                Validatestr = "驗證碼錯誤，請重新確認";
            }
            return Validatestr;
        }
        public string LoginCheck(LoginDTO Value)
        {

            Members LoginMember = GetDataEmail(Value.Email);

            if (LoginMember != null)
            {
                if (String.IsNullOrWhiteSpace(LoginMember.AuthCode))
                {
                    if (PasswordCheck(LoginMember, Value.Password))
                    {

                        return "";
                    }
                    else
                    {
                        return " 密碼輸入錯誤 ";
                    }
                }
                else
                {
                    return " 此帳號尚未經過 Email 驗證，請去收信 ";
                }
            }
            else
            {
                return " 無此會員帳號，請去註冊 ";
            }
        }
        public bool PasswordCheck(Members CheckMember, string Password)
        {
            string pwd = HashPassword(Password);
            bool result = CheckMember.Password.Equals(pwd);
            return result;
        }
        public void updatePassword(Members members, string Password)
        {

            if (members != null)
            {
                members.Password = HashPassword(Password);
                _MembersRepository.Update(members);
                
            }
        }
        public string ChangePassword(string email, string Password, string NEWPassword)
        {
            Members members = GetDataEmail(email);
            if (PasswordCheck(members, Password))
            {

                members.Password = HashPassword(NEWPassword);
                _MembersRepository.Update(members);
                return "修改成功";
            }
            return "密碼不正確";
        }

        public ProfileDTO profile(string email)
        {
            var member = _OnlineBookClubContext.Members
            .Where(m => m.Email == email)
            .Select(m => new ProfileDTO
            {
                Name = m.UserName,
                Birthday = m.Birthday,
                Gender = m.Gender,
                ProfilePictureUrl = m.ProfilePictureUrl
            })
            .SingleOrDefault();  // 確保只取得一筆資料

            return member;  
        }
        public bool UpdateProfile(ProfileDTO profileDto,string email, string savedFilePath)
        {
            var member = GetDataEmail(email);

            if (member == null)
            {
                return false; // 找不到會員
            }

            // 更新會員資料
            member.UserName = profileDto.Name;
            member.Birthday = profileDto.Birthday;
            member.Gender = profileDto.Gender;
            member.ProfilePictureUrl = savedFilePath;
            _MembersRepository.Update(member);
            return true;
        }
    }
}
