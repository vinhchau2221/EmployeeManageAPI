using Employee.api.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Employee.api.Service
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;      // at least 32 characters
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryMinutes { get; set; } = 60;
    }

    public class TokenService
    {
        private readonly JwtSettings _settings;
        public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

        public (string Token, DateTime ExpiresAtUtc) CreateToken(EmployeeModel employee)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.employeeId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, employee.email),
                new Claim(ClaimTypes.Name, employee.name),
                new Claim("designationId", employee.designationId.ToString())
            };

            var expires = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return (new JwtSecurityTokenHandler().WriteToken(jwt), expires);
        }
    }

}
