using FirstSparrow.Application.Features.Restaurant.ConfirmOwnerPhoneCommand;
using FirstSparrow.Application.Features.Restaurant.LoginCommand;
using FirstSparrow.Application.Features.Restaurant.RegisterCommand;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/restaurant")]
public class RestaurantController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request, cancellationToken));
    }

    [HttpPost("confirm-owner-phone-number")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmOwnerPhoneNumber([FromBody] ConfirmOwnerPhoneCommand request, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(request, cancellationToken));
    }
}