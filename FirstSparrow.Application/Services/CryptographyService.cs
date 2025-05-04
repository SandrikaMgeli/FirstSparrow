using System.Security.Cryptography;
using System.Text;
using FirstSparrow.Application.Services.Abstractions;

namespace FirstSparrow.Application.Services;

public class CryptographyService : ICryptographyService
{
    public string Hash(string text)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(text);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLower();
    }
}