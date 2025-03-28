using Microsoft.IdentityModel.Tokens;
using OnlineBookClub.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineBookClub.Token
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJwtToken(Members user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("FullName", user.UserName),
            new Claim(JwtRegisteredClaimNames.NameId, user.User_Id.ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:KEY"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public void SetJwtCookie(HttpResponse response, string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // 避免 JavaScript 讀取，防止 XSS 攻擊
            Secure = false,   // 僅在 HTTPS 傳輸（開發環境可設 false）
            SameSite = SameSiteMode.Strict, // 限制跨站請求
            Expires = DateTime.Now.AddMinutes(30)
        };
        response.Cookies.Append("JWT", token, cookieOptions);
    }
    }
}
