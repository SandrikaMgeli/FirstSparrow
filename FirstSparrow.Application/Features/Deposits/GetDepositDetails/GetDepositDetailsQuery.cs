using MediatR;

namespace FirstSparrow.Application.Features.Deposits.GetDepositDetails;

public class GetDepositDetailsQuery : IRequest<GetDepositDetailsResponse>
{
    private string _commitment;

    public string Commitment
    {
        get => _commitment;
        set => _commitment = "0x" + value.ToUpper().AsSpan(2).ToString();
    }
}