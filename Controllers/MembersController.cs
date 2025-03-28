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
        public MembersController(IWebHostEnvironment _env, MembersService _MemberService, MailService _MailService, OnlineBookClubContext _OnlineBookClubContext, IConfiguration config)
        {
            env = _env;
            MemberService = _MemberService;
            MailService = _MailService;
            OnlineBookClubContext = _OnlineBookClubContext;
            _config = config;

        }
        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterDTO Register)
        {
            if (ModelState.IsValid && MemberService.EmailCheck(Register.newMember.Email))
            {
                Register.newMember.Password = MemberService.HashPassword(Register.Password);
                Register.newMember.AuthCode = MailService.GetAuthCode();
                MemberService.Register(Register.newMember);
                string TempMail = System.IO.File.ReadAllText(Path.Combine(env.ContentRootPath, "RegisterEmailTemplate.html"));
                string ValidateUrl = $"{Request.Scheme}://{Request.Host}/api/members/EmailValidate?Email={Register.newMember.Email}&AuthCode={Register.newMember.AuthCode}";
                string MailBody = MailService.GetRegisterMailBody(TempMail, Register.newMember.UserName, ValidateUrl);
                MailService.SendRegisterMail(MailBody, Register.newMember.Email);
                return Ok("註冊成功，請去收信以驗證Email");
            }
            return BadRequest("註冊失敗，請重新註冊");
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
                var claims = new List<Claim>    
                {
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim("FullName", user.UserName),
                    new Claim(JwtRegisteredClaimNames.NameId, user.User_Id.ToString())
                };
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:KEY"]));


                //設定jwt相關資訊
                var jwt = new JwtSecurityToken
                (
                    issuer: _config["JWT:Issuer"],
                    audience: _config["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                //產生JWT Token
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,  // 避免 JavaScript 讀取，防止 XSS 攻擊
                    Secure = false,    // 僅在 HTTPS 傳輸
                    SameSite = SameSiteMode.Strict, // 限制跨站請求
                    Expires = DateTime.Now.AddMinutes(30)
                };
                Response.Cookies.Append("JWT", token, cookieOptions);
                return Ok(); // 已登入則重新導向

            }
            else
            {
                // 有驗證錯誤訊息，加入頁面模型中
                ModelState.AddModelError("", ValidateStr);
                // 將資料回填至 View 中
                return BadRequest(ValidateStr);
            }
        }
       
        
        [HttpDelete]
        public ActionResult logout()
        {
            Response.Cookies.Delete("JWT");
            return Ok("登出成功");

        }
        [AllowAnonymous]
        [HttpGet("NoLogin")]
        public ActionResult noLogin()
        {
            return Ok("為登入");
        }
    }
}
