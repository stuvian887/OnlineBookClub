using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using OnlineBookClub.Services;
using System.Security.Claims;
using OnlineBookClub.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OnlineBookClub.Token;
using Microsoft.AspNetCore.Identity;
using Azure;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineBookClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IWebHostEnvironment env;
        private readonly MembersService MemberService;
        private readonly MailService MailService;
        private readonly OnlineBookClubContext OnlineBookClubContext;
        private readonly IConfiguration _config;
        private readonly JwtService _jwtService;
        public MembersController(IWebHostEnvironment _env, MembersService _MemberService, MailService _MailService, OnlineBookClubContext _OnlineBookClubContext, IConfiguration config, JwtService jwtService)
        {
            env = _env;
            MemberService = _MemberService;
            MailService = _MailService;
            OnlineBookClubContext = _OnlineBookClubContext;
            _config = config;
            _jwtService = jwtService;
        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterDTO Register)
        {
            if (ModelState.IsValid && MemberService.EmailCheck(Register.Email))
            {
                Register.Password = MemberService.HashPassword(Register.Password);
                string authcode = MailService.GetAuthCode();
                var newmember = new Members()
                {
                    Email = Register.Email,
                    UserName=Register.UserName,
                    AuthCode=authcode,
                    Password = Register.Password
                };
                MemberService.Register(newmember);
                string TempMail = System.IO.File.ReadAllText(Path.Combine(env.ContentRootPath, "RegisterEmailTemplate.html"));
                string ValidateUrl = $"{Request.Scheme}://{Request.Host}/api/members/EmailValidate?Email={Register.Email}&AuthCode={authcode}";
                string MailBody = MailService.GetRegisterMailBody(TempMail, Register.UserName, ValidateUrl);
                MailService.SendRegisterMail(MailBody, Register.Email);
                
                return Ok(new { str = "註冊成功，請去收信以驗證Email" });
            }

            return BadRequest(new { str = "帳號已註冊請重新註冊" });
        }
        [HttpGet("EmailValidate")]
        [AllowAnonymous]
        public IActionResult EmailValidate(string Email, string AuthCode)
        {
            string Validatestr = MemberService.EmailValidate(Email, AuthCode);
            return Ok(Validatestr);
        }
        [HttpPost("Login")] // 設定此 Action 只接受頁面 POST 資料傳入
        [AllowAnonymous]
        public ActionResult Login(LoginDTO LoginMember)
        {

            string ValidateStr = MemberService.LoginCheck(LoginMember);
            // 判斷驗證後結果是否有錯誤訊息
            if (String.IsNullOrEmpty(ValidateStr))
            {
                var user = (from a in OnlineBookClubContext.Members
                            where a.Email == LoginMember.Email
                            && a.Password == MemberService.HashPassword( LoginMember.Password)
                            select a).SingleOrDefault();
                // 使用 JwtService 產生 Token   
                string token = _jwtService.GenerateJwtToken(user);
               
                // 設定 Cookie
                _jwtService.SetJwtCookie(Response, token);
                return Ok(new { token }); // 已登入則重新導向

            }
            else
            {
                // 有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                // 將資料回填至 View 中
                return BadRequest(new { ValidateStr });
            }
        }
       
        
        [HttpDelete("logout")]
        public ActionResult logout()
        {
            
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // 避免 JavaScript 讀取，防止 XSS 攻擊
                Secure = true,   // 僅在 HTTPS 傳輸（開發環境可設 false）
                SameSite = SameSiteMode.None, // 限制跨站請求
                Expires = DateTime.Now.AddMinutes(30)
           ,
                Path = "/"

            };
            
            Response.Cookies.Delete("JWT",cookieOptions);
            return Ok("登出成功");

        }

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public ActionResult ForgetPassword([FromForm] string emaill) 
        {
            if (!MemberService.EmailCheck(emaill)) 
            {
                string authcode = MailService.GetAuthCode();
                var member = MemberService.GetDataEmail(emaill);
                MemberService.authcode(emaill, authcode);
                string TempMail = System.IO.File.ReadAllText(Path.Combine(env.ContentRootPath, "forgetpasswordpage.html"));
                string ValidateUrl = $"{Request.Scheme}://{Request.Host}/api/members/PSValidate?Email={emaill}&AuthCode={authcode}";
                
                string MailBody = MailService.GetRegisterMailBody(TempMail, member.UserName, ValidateUrl);
                MailService.SendRegisterMail(MailBody, emaill);
                string str1 = "輸入成功，請去收信以驗證Email";
                return Ok(new { str1 });
            }
            string str = "無此帳號";
            return BadRequest(new { str });
        }
        [HttpGet("PSValidate")]
        [AllowAnonymous]
        public IActionResult PSValidate(string Email, string AuthCode)
        {
            string Validatestr = MemberService.forgetpsEmailValidate(Email, AuthCode);

            return Ok(Validatestr);
        }
        [HttpPut("changeps")]
        [AllowAnonymous]
        public IActionResult changeps(string emaill,string ps,string ps2)
        {
            
            
            
            if (ps == ps2)
            {
                var member = MemberService.GetDataEmail(emaill);
                if (member != null)
                {
                MemberService.updatePassword(member, ps);
                return Ok("修改成功 請重新登入");
                }
                else
                {
                    return BadRequest("資訊錯誤請重新驗證");
                }
            }
            else
            {
               
                return BadRequest("密碼不同請重新輸入");
            }
            
           
        }


        [HttpPut("changePassword")]
        public IActionResult ChangPS(ChengePasswordDTO cp)
        {


            var token = HttpContext.Request.Cookies["JWT"];
            var email = _jwtService.GetemailFromToken(token);
            var member = MemberService.GetDataEmail(email);

            if (member.Password != MemberService.HashPassword(cp.oldpassword))
            {
                return BadRequest("舊密碼不同請重新輸入");
            }
            if (cp.newpassword != cp.Chenckpassword) { return BadRequest("新密碼與確認密碼不同，請重新輸入"); }
            
            MemberService.ChangePassword(email, cp.oldpassword, cp.newpassword);


            return Ok("修改成功 請重新登入");
        }
        [HttpGet("Get-profile")]
        public IActionResult profile() 
        {
            var token = HttpContext.Request.Cookies["JWT"];
            var email = _jwtService.GetemailFromToken(token);
            var member = MemberService.profile(email);
            if (member == null)
            {
                return NotFound("找不到會員資料");
            }

            return Ok(member);
            
        }
        [HttpPut("Update-profile")]
        public IActionResult Updateprofile([FromForm] ProfileDTO data, ICollection<IFormFile> files)
        {
            var rootPath = env.ContentRootPath + "\\wwwroot\\";
            var token = HttpContext.Request.Cookies["JWT"];
            var email = _jwtService.GetemailFromToken(token);
            string savedFilePath = null;

            // 如果上傳了圖片
            if (data.ProfilePictureUrl != null && files.Count > 0)
            {
                var uploadFolder = Path.Combine(rootPath, "Members/imgs");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var file = files.First(); // 只用第一張圖
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    savedFilePath = "/Members/imgs/" + fileName;
                }
            }

            bool result = MemberService.UpdateProfile(data, email, savedFilePath);

            if (!result)
            {
                return NotFound(new { message = "找不到會員，更新失敗" });
            }

            return Ok(new { message = "會員資料更新成功" });
        }





    }
}
