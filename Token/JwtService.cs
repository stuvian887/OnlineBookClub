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
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

        public void SetJwtCookie(HttpResponse response, string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,  // 避免 JavaScript 讀取，防止 XSS 攻擊
            Secure = true,   // 僅在 HTTPS 傳輸（開發環境可設 false）
            SameSite = SameSiteMode.None, // 限制跨站請求
            Expires = DateTime.Now.AddMinutes(9999)
            ,Path = "/"

        };
        response.Cookies.Append("JWT", token, cookieOptions);
    }

        public string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JWT:Key"]);
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["jwt:Issuer"],  // 從配置中讀取 Issuer
                ValidAudience = _config["jwt:Audience"],  // 確保這裡的 "User" 與 token 中的 aud 聲明一致
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)  // 使用對稱密鑰驗證簽名
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // 从 JWT token 中提取 User_Id
            var userIdClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.NameId).Value;
            return userIdClaim; // 返回 UserId
        }
        public string GetNameFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JWT:Key"]);
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["jwt:Issuer"],  // 從配置中讀取 Issuer
                ValidAudience = _config["jwt:Audience"],  // 確保這裡的 "User" 與 token 中的 aud 聲明一致
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)  // 使用對稱密鑰驗證簽名
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            
            var userIdClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value;
            return userIdClaim; // 返回 UserId
        }
        public string GetemailFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["JWT:Key"]);
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _config["jwt:Issuer"],  // 從配置中讀取 Issuer
                ValidAudience = _config["jwt:Audience"],  // 確保這裡的 "User" 與 token 中的 aud 聲明一致
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)  // 使用對稱密鑰驗證簽名
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            
            var userIdClaim = jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value;
            return userIdClaim; // 返回 UserId
        }

    }
}
