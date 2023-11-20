using CatalogoApiMinimalAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CatalogoApiMinimalAPI.Services
{
    public class TokenService : ITokenService
    {
        public string GerarToken(string key, string issuer, string audience, User user)
        {
            Claim[] claims = new[]
            {
               new Claim(ClaimTypes.Name, user.UserName),
               new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
           };

            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(key));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken token = new(issuer: issuer, audience: audience,claims:claims, expires: DateTime.Now.AddMinutes(10),signingCredentials:credentials);

            JwtSecurityTokenHandler handler = new();
            return handler.WriteToken(token);
        }
    }
}
