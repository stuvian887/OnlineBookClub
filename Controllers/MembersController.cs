using Microsoft.AspNetCore.Mvc;
using OnlineBookClub.DTO;
using OnlineBookClub.Service;
using OnlineBookClub.Services;

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
        
        public MembersController(IWebHostEnvironment _env, MembersService _MemberService, MailService _MailService)
        {
            env = _env;
            MemberService = _MemberService;
            MailService = _MailService;
        }
        [HttpPost("Register")]
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
        public IActionResult EmailValidate(string Email, string AuthCode)
        {
            string Validatestr = MemberService.EmailValidate(Email, AuthCode);
            return Ok(Validatestr);
        }
    }
}
