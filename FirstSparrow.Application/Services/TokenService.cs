using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Shared;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FirstSparrow.Application.Services;

public class TokenService(IOptions<IdentitySettings> identitySettings) : ITokenService
{
    public string GenerateToken(Dictionary<string, string> claimsMapping, TimeSpan validForTimespan)
    {
        var credentials = GetSigningCredentials();

        IEnumerable<Claim> claims = GenerateClaims(claimsMapping);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: identitySettings.Value.Issuer,
            audience: identitySettings.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(validForTimespan),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static IEnumerable<Claim> GenerateClaims(Dictionary<string, string> claimsMapping) => 
        claimsMapping.Select(claim => new Claim(claim.Key, claim.Value));

    private SigningCredentials GetSigningCredentials()
    {
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(identitySettings.Value.SecretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);
        return credentials;
    }
}