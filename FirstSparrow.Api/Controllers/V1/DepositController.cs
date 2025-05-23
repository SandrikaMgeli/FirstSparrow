using FirstSparrow.Application.Features.Deposits.GetDepositDetails;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/deposit/")]
public class DepositController(IMediator mediator) : ControllerBase
{
    [HttpGet("details/{commitment}")]
    public async Task<IActionResult> GetDepositDetails([FromRoute] string commitment, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetDepositDetailsQuery(){ Commitment = commitment }, cancellationToken));
    }
}