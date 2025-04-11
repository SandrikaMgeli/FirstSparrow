using FirstSparrow.Api.Controllers.V1.Enums;

namespace FirstSparrow.Api.Controllers.V1.Models;

public class DiscordInteractionRequest
{
    public DiscordInteractionType Type { get; set; }
}