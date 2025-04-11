using System.ComponentModel.DataAnnotations;

namespace FirstSparrow.Api.Settings;

public class DiscordSettings
{
    [Required]
    public string InteractionsPublicKey { get; set; }

    [Required]
    public string SignatureHeaderKey { get; set; }

    [Required]
    public string TimestampHeaderKey { get; set; }
}