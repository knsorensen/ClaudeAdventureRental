using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AdventureRental.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AdventureRental.Infrastructure.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration) => _configuration = configuration;

    public string CreateToken(ApplicationUser user, IList<string> roles)
    {
        var secret = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"] ?? "AdventureRental",
            audience: _configuration["JwtSettings:Audience"] ?? "AdventureRentalUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                double.TryParse(_configuration["JwtSettings:ExpirationHours"], out var h) ? h : 24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
