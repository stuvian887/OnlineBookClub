using OnlineBookClub.Models;
using System.Security.Cryptography;
using System.Text;
using OnlineBookClub.Repository;
using OnlineBookClub.DTO;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
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
        public async Task<Members> GetUserById(int userid)
        {
            return await _MembersRepository.GetByIdAsync(userid);
        }

        public async Task RegisterAsync(Members NewMember)
        {
            await _MembersRepository.AddAsync(NewMember);
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
        public async Task<Members> GetDataEmailAsync(string Email)
        {
            return await _OnlineBookClubContext.Members
                .Where(m => m.Email == Email)
                .SingleOrDefaultAsync(); // 非同步查詢
        }
        public async Task<bool> EmailCheckAsync(string Email)
        {
            Members Data = await GetDataEmailAsync(Email);
            return (Data == null);
        }
        public async Task<string> EmailValidateAsync(string Email, string AuthCode)
        {
            Members ValidateMember = await GetDataEmailAsync(Email);
            string Validatestr = string.Empty;
            if (ValidateMember != null && ValidateMember.AuthCode == AuthCode)
            {
                var update = await _OnlineBookClubContext.Members
                    .Where(a => a.Email == ValidateMember.Email)
                    .SingleOrDefaultAsync();
                update.AuthCode = string.Empty;  // 清空驗證碼，標記為已驗證
                await _OnlineBookClubContext.SaveChangesAsync(); // 非同步儲存變更
                return "帳號信箱驗證成功，現在可以登入了";
            }
            else
            {
                Validatestr = "驗證碼錯誤，請重新確認或再註冊";
            }
            return Validatestr;
        }
        public async Task authcodeAsync(string email, string authcode)
        {
            var members = await GetDataEmailAsync(email);
            members.AuthCode = authcode;
            await _MembersRepository.UpdateAsync(members);
        }
        public async Task<string> forgetpsEmailValidateAsync(string Email, string AuthCode)
        {
            Members ValidateMember = await GetDataEmailAsync(Email);
            string Validatestr = string.Empty;
            if (ValidateMember != null && ValidateMember.AuthCode == AuthCode)
            {
                var update = await _OnlineBookClubContext.Members
                    .Where(a => a.Email == ValidateMember.Email)
                    .SingleOrDefaultAsync();
                update.AuthCode = string.Empty;  // 清空驗證碼，標記為已驗證
                await _OnlineBookClubContext.SaveChangesAsync(); // 非同步儲存變更
                string redirectUrl = $"http://127.0.0.1:5500/Login/newpassword.html?email={Uri.EscapeDataString(Email)}";

                return "帳號信箱驗證成功請回到剛剛頁面修改密碼 \n" + redirectUrl;
            }
            else
            {
                Validatestr = "驗證碼錯誤，請重新確認";
            }
            return Validatestr;
        }
        public async Task<string> LoginCheckAsync(LoginDTO Value)
        {
            Members LoginMember = await GetDataEmailAsync(Value.Email);

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
        public async Task updatePasswordAsync(Members members, string Password)
        {
            if (members != null)
            {
                members.Password = HashPassword(Password);
                await _MembersRepository.UpdateAsync(members);
            }
        }
        public async Task<string> ChangePasswordAsync(string email, string Password, string NEWPassword)
        {
            Members members = await GetDataEmailAsync(email);
            if (PasswordCheck(members, Password))
            {
                members.Password = HashPassword(NEWPassword);
                await _MembersRepository.UpdateAsync(members);
                return "修改成功";
            }
            return "密碼不正確";
        }

        public async Task<ProfileDTO> profileAsync(string email)
        {
            var member = await _OnlineBookClubContext.Members
                .Where(m => m.Email == email)
                .Select(m => new ProfileDTO
                {
                    Name = m.UserName,
                    Birthday = m.Birthday,
                    email = m.Email,
                    Gender = m.Gender,
                    ProfilePictureUrl = m.ProfilePictureUrl
                })
                .SingleOrDefaultAsync();  // 非同步查詢

            return member;
        }

        public async Task<bool> UpdateProfileAsync(ProfileDTO profileDto, string email, string savedFilePath)
        {
            var member = await GetDataEmailAsync(email);

            if (member == null)
            {
                return false; // 找不到會員
            }

            // 更新基本資料
            member.UserName = profileDto.Name;
            member.Birthday = profileDto.Birthday;
            member.Gender = profileDto.Gender;

            // 如果有上傳新圖片，才更新圖片網址
            if (!string.IsNullOrEmpty(savedFilePath))
            {
                member.ProfilePictureUrl = savedFilePath;
            }

            await _MembersRepository.UpdateAsync(member);
            return true;
        }
    }
}
