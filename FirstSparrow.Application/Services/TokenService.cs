using FirstSparrow.Application.Services.Abstractions;

namespace FirstSparrow.Application.Services;

public class TokenService : ITokenService
{
    public string GenerateToken(Dictionary<string, string> claims)
    {
        throw new NotImplementedException();
    }
}