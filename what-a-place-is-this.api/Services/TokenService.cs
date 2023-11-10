using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using what_a_place_is_this.api.Models;

namespace what_a_place_is_this.api.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _service;

        public TokenService(IConfiguration configuration, UserService service)
        {
            _configuration = configuration;
            _service = service;
        }

        public async Task<string> GenerateToken(UserModel user)
        {
            UserModel _user = new();
            _user = await _service.Login(user);
            if (_user is null) return string.Empty;

            var _secretKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var signinCredentials = new SigningCredentials
                (_secretKey, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: new[] {
                new Claim(ClaimTypes.Name, _user.UserName),
                new Claim(ClaimTypes.Role, _user.Role)
                },
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signinCredentials
                );
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }
    }
}
