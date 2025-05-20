using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
using FirstSparrow.Application.Shared;
using FirstSparrow.Infrastructure.Services.Ethereum.Models;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace FirstSparrow.Infrastructure.Services.Ethereum;

public class EthereumService : IBlockChainService
{
    private readonly Web3 _web3;
    private readonly Contract _smartContract;

    public EthereumService(IOptions<FirstSparrowConfigs> firstSparrowConfigs, IHttpClientFactory httpClientFactory)
    {
        _web3 = CreateWeb3Client(firstSparrowConfigs, httpClientFactory);
        _smartContract = _web3.Eth.GetContract(EthereumServiceConstants.DepositEventABI, firstSparrowConfigs.Value.SmartContractAddress);
    }

    public async Task<List<Deposit>> FetchDeposits(FetchDepositsParams @params, CancellationToken cancellationToken = default)
    {
        IEnumerable<DepositEvent> events = await FetchDepositEvents(@params, cancellationToken);
        return events.Select(@event => new Deposit()
        {
            Commitment = "0x" + Convert.ToHexString(@event.Commitment ?? throw new AppException("Deposit's commitment was null", ExceptionCode.GENERAL_ERROR)),
            CreateTimestamp = DateTime.Now,
            LeafIndex = @event.LeafIndex,
        }).ToList();
    }

    private async Task<IEnumerable<DepositEvent>> FetchDepositEvents(FetchDepositsParams @params, CancellationToken cancellationToken = default)
    {
        Event depositEvent = _smartContract.GetEvent(DepositEventConstants.EventName);
        NewFilterInput filterInput = await GenerateFilter(@params, depositEvent, cancellationToken);

        List<EventLog<DepositEvent>> eventLogs = await depositEvent.GetAllChangesAsync<DepositEvent>(filterInput);
        return eventLogs.Select(eventLog => eventLog.Event);
    }

    private async Task<NewFilterInput> GenerateFilter(FetchDepositsParams @params, Event @event, CancellationToken cancellationToken)
    {
        ulong latestBlock = await GetLatestBlockNumber(cancellationToken);

        ulong toBlock = Math.Min(@params.FromBlock + (ulong)@params.BatchSize, latestBlock);

        return @event.CreateFilterInput(
            fromBlock: new BlockParameter(@params.FromBlock),
            toBlock: new BlockParameter(toBlock)
        );
    }

    private async Task<ulong> GetLatestBlockNumber(CancellationToken cancellationToken = default)
    {
        HexBigInteger latestBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync(cancellationToken);
        return ulong.Parse(latestBlock.Value.ToString());
    }

    private static Web3 CreateWeb3Client(IOptions<FirstSparrowConfigs> firstSparrowConfigs, IHttpClientFactory httpClientFactory)
    {
        HttpClient client = httpClientFactory.CreateClient("ethereum_rpc");
        SimpleRpcClient rpcClient = new SimpleRpcClient(new Uri(firstSparrowConfigs.Value.RpcUrl), client);
        return new Web3(rpcClient);
    }
}

public static class EthereumServiceConstants
{
    internal const string DepositEventABI = @"
        [{
            ""anonymous"": false,
            ""inputs"": [
                {
                    ""indexed"": true,
                    ""name"": ""commitment"",
                    ""type"": ""bytes32""
                },
                {
                    ""indexed"": false,
                    ""name"": ""leafIndex"",
                    ""type"": ""uint32""
                },
                {
                    ""indexed"": false,
                    ""name"": ""timestamp"",
                    ""type"": ""uint256""
                }
            ],
            ""name"": ""Deposit"",
            ""type"": ""event""
        }]";
}