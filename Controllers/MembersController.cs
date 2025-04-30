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
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> Register([FromBody] RegisterDTO Register)
        {
            if (ModelState.IsValid && await MemberService.EmailCheckAsync(Register.Email))
            {
                Register.Password = MemberService.HashPassword(Register.Password);
                string authcode = MailService.GetAuthCode();
                var newmember = new Members()
                {
                    Email = Register.Email,
                    UserName = Register.UserName,
                    AuthCode = authcode,
                    Password = Register.Password
                };

                await MemberService.RegisterAsync(newmember); // Non-blocking call

                string TempMail = await System.IO.File.ReadAllTextAsync(Path.Combine(env.ContentRootPath, "RegisterEmailTemplate.html")); // Asynchronous file reading
                string ValidateUrl = $"{Request.Scheme}://{Request.Host}/api/members/EmailValidate?Email={Register.Email}&AuthCode={authcode}";
                string MailBody = MailService.GetRegisterMailBody(TempMail, Register.UserName, ValidateUrl);
                await MailService.SendRegisterMail(MailBody, Register.Email); // Non-blocking mail sending

                return Ok(new { str = "註冊成功，請去收信以驗證Email" });
            }

            return BadRequest(new { str = "帳號已註冊請重新註冊" });
        }

        [HttpGet("EmailValidate")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailValidate(string Email, string AuthCode)
        {
            string Validatestr = await MemberService.EmailValidateAsync(Email, AuthCode); // Non-blocking call
            return Ok(Validatestr);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDTO LoginMember)
        {
            string ValidateStr = await MemberService.LoginCheckAsync(LoginMember); // Non-blocking call

            if (String.IsNullOrEmpty(ValidateStr))
            {
                var user = await (from a in OnlineBookClubContext.Members
                                  where a.Email == LoginMember.Email
                                  && a.Password == MemberService.HashPassword(LoginMember.Password)
                                  select a).SingleOrDefaultAsync(); // Async query

                string token = _jwtService.GenerateJwtToken(user);
                _jwtService.SetJwtCookie(Response, token);
                return Ok(new { token });
            }
            else
            {
                ModelState.AddModelError("", ValidateStr);
                return BadRequest(new { ValidateStr });
            }
        }

        [HttpDelete("logout")]
        public IActionResult logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddMinutes(30),
                Path = "/"
            };

            Response.Cookies.Delete("JWT", cookieOptions);
            return Ok("登出成功");
        }

        [HttpPost("ForgetPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgetPassword([FromForm] string emaill)
        {
            if (!await MemberService.EmailCheckAsync(emaill)) // Non-blocking call
            {
                string authcode = MailService.GetAuthCode();
                var member = await MemberService.GetDataEmailAsync(emaill); // Async call
                await MemberService.authcodeAsync(emaill, authcode); // Async call

                string TempMail = await System.IO.File.ReadAllTextAsync(Path.Combine(env.ContentRootPath, "forgetpasswordpage.html"));
                string ValidateUrl = $"{Request.Scheme}://{Request.Host}/api/members/PSValidate?Email={emaill}&AuthCode={authcode}";
                string MailBody = MailService.GetRegisterMailBody(TempMail, member.UserName, ValidateUrl);
                await MailService.SendRegisterMail(MailBody, emaill); // Async mail sending

                return Ok(new { str1 = "輸入成功，請去收信以驗證Email" });
            }
            string str = "無此帳號";
            return BadRequest(new { str });
        }

        [HttpGet("PSValidate")]
        [AllowAnonymous]
        public async Task<IActionResult> PSValidate(string Email, string AuthCode)
        {
            string Validatestr = await MemberService.forgetpsEmailValidateAsync(Email, AuthCode); // Non-blocking call
            return Ok(Validatestr);
        }

        [HttpPut("changeps")]
        [AllowAnonymous]
        public async Task<IActionResult> changeps(string emaill, string ps, string ps2)
        {
            if (ps == ps2)
            {
                var member = await MemberService.GetDataEmailAsync(emaill); // Non-blocking call
                if (member != null)
                {
                    await MemberService.updatePasswordAsync(member, ps); // Async password update
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
        public async Task<IActionResult> ChangPS(ChengePasswordDTO cp)
        {
            var token = HttpContext.Request.Cookies["JWT"];
            var email = _jwtService.GetemailFromToken(token);
            var member = await MemberService.GetDataEmailAsync(email); // Async call

            if (member.Password != MemberService.HashPassword(cp.oldpassword))
            {
                return BadRequest("舊密碼不同請重新輸入");
            }

            if (cp.newpassword != cp.Chenckpassword)
            {
                return BadRequest("新密碼與確認密碼不同，請重新輸入");
            }

            await MemberService.ChangePasswordAsync(email, cp.oldpassword, cp.newpassword); // Async password change
            return Ok("修改成功 請重新登入");
        }

        [HttpGet("Get-profile")]
        public async Task<IActionResult> profile()
        {
            var token = HttpContext.Request.Cookies["JWT"];
            var email = _jwtService.GetemailFromToken(token);
            var member = await MemberService.profileAsync(email); // Async profile fetch
            if (member == null)
            {
                return NotFound("找不到會員資料");
            }

            return Ok(member);
        }

        [HttpPut("Update-profile")]
        public async Task<IActionResult> Updateprofile([FromForm] ProfileDTO data)
        {
            var token = HttpContext.Request.Cookies["JWT"];
            var email = _jwtService.GetemailFromToken(token);

            string savedFilePath = null;

            if (data.ProfilePicture != null)
            {
                var uploadFolder = Path.Combine(env.ContentRootPath, "wwwroot", "Members", "imgs");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var file = data.ProfilePicture;
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream); // Async file copy
                    }

                    var request = HttpContext.Request;
                    var hosturl = $"{request.Scheme}://{request.Host}";
                    savedFilePath = $"{hosturl}/Members/imgs/{fileName}";
                }
            }

            bool result = await MemberService.UpdateProfileAsync(data, email, savedFilePath); // Async profile update

            if (!result)
            {
                return NotFound(new { message = "找不到會員，更新失敗" });
            }

            return Ok(new { message = "會員資料更新成功" });
        }
    }
}
