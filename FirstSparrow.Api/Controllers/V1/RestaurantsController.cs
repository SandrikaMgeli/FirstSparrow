using FirstSparrow.Application.Features.Restaurant.LoginCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class RestaurantsController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken cancellationToken)
    {
        LoginResponse response = await mediator.Send(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register()
    {
        return Ok();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        return Ok();
    }
}