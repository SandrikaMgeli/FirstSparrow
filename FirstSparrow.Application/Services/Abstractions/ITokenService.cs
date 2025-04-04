namespace FirstSparrow.Application.Services.Abstractions;

public interface ITokenService
{
    string GenerateToken(Dictionary<string, string> claims, TimeSpan validForTimespan);
}