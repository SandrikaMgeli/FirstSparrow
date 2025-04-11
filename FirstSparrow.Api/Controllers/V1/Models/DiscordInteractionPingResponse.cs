using FirstSparrow.Api.Controllers.V1.Enums;

namespace FirstSparrow.Api.Controllers.V1.Models;

/// <summary>
/// This is response, which will be returned when discord sends ping request.
/// </summary>
public class DiscordInteractionPingResponse
{
    public DiscordInteractionType Type { get; set; }
}