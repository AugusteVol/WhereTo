using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WhereTo.Api.Dtos;
using WhereTo.Api.Models;

namespace WhereTo.Api.Security;

public class JwtTokenService(IConfiguration configuration)
{
    public AuthResponse Create(Account account)
    {
        var key = configuration["Jwt:Key"] //signature - ar sukurtas mano api, nebuvo pakeistas 
            ?? throw new InvalidOperationException("Nerasta Jwt:Key konfigūracija.");
        var expiresAt = DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpiresMinutes", 5));

        var claims = new[] //payload - info
        {
            new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.Email),
            new Claim(ClaimTypes.Role, account.Role)
        };

        var credentials = new SigningCredentials( //header - kaip sukurtas
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token), ExpiresAt = expiresAt,
            Account = new AccountResponse
            {
                Id = account.Id,
                Name = account.Name,
                Email = account.Email
            }
        };
    }
}
