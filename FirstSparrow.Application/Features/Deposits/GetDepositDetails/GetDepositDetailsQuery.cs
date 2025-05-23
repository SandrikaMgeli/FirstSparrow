using MediatR;

namespace FirstSparrow.Application.Features.Deposits.GetDepositDetails;

public class GetDepositDetailsQuery : IRequest<GetDepositDetailsResponse>
{
    public string Commitment { get; set; }
}