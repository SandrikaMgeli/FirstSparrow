using System.ComponentModel.DataAnnotations;

namespace FirstSparrow.Application.Shared;

public class IdentitySettings
{
    [Required]
    public string Issuer { get; set; }

    [Required]
    public string Audience { get; set; }

    [Required]
    public string SecretKey { get; set; }

    [Required]
    public int AccessTokenExpirationInMinutes { get; set; }

    [Required]
    public int RefreshTokenExpirationInDays { get; set; }
}