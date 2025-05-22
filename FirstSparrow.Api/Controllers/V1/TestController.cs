using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FirstSparrow.Api.Controllers.V1;

[ApiController]
[Route("api/v1/test/process-deposit")]
public class TestController(IDepositService depositService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ProcessDeposit([FromBody] Deposit deposit, CancellationToken cancellationToken)
    {
        await depositService.ProcessDeposit(deposit, cancellationToken);
        return Ok();
    }
}