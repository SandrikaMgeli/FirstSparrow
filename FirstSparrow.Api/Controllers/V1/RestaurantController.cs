using FirstSparrow.Application.Features.Restaurant.RegisterRestaurantCommand;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/restaurant")]
public class RestaurantController(
    IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRestaurantCommand registerRestaurantCommand)
    {
        return Ok(await  mediator.Send(registerRestaurantCommand));
    }
}