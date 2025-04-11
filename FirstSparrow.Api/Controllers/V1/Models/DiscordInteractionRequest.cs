using System.Text.Json.Serialization;
using FirstSparrow.Api.Controllers.V1.Enums;

namespace FirstSparrow.Api.Controllers.V1.Models;

public class DiscordInteractionRequest
{
    [JsonPropertyName("type")]
    public DiscordInteractionType Type { get; set; }
}