using FirstSparrow.Application.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FirstSparrow.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController(TimeProvider timeProvider) : ControllerBase
{
    [HttpGet]
    public IActionResult CheckHealth()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = timeProvider.GetUtcNowDateTime(),
            service = "FirstSparrow.Api"
        });
    }
}