namespace FirstSparrow.Application.Features.Deposits.GetDepositDetails;

public class GetDepositDetailsResponse
{
    public List<string> Path { get; set; }

    public List<int> Indices { get; set; }

    public string Root { get; set; }

    public long Index { get; set; }
}