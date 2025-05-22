using FirstSparrow.Application.Domain.Models;

namespace FirstSparrow.Application.Services.Abstractions;

public interface IDepositService
{
    Task ProcessDeposit(Deposit merkleNode, CancellationToken cancellationToken);
}