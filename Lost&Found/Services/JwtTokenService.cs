using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Lost_Found.Models;
using Microsoft.IdentityModel.Tokens;
using Claim = System.Security.Claims.Claim;
using ClaimTypes = System.Security.Claims.ClaimTypes;

namespace Lost_Found.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Jwt:Key is not configured.");
            }

            var role = user is Admin ? "Admin" : "StandardUser";

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var expiresInMinutes = jwtSection.GetValue<int?>("ExpiresInMinutes") ?? 120;

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
