namespace FirstSparrow.Application.Services.Models;

public class FetchDepositsParams
{
    public ulong FromBlock { get; set; }

    public int BatchSize { get; set; }
}