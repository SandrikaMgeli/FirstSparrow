using MediatR;

namespace FirstSparrow.Application.Features.Deposits.SyncDeposits;

public class SyncDepositsCommandHandler() : IRequestHandler<SyncDepositsCommand, SyncDepositsResponse>
{
    public Task<SyncDepositsResponse> Handle(SyncDepositsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}