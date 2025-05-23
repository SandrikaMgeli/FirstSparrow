using System.Numerics;
using FirstSparrow.Application.Domain.Exceptions;
using FirstSparrow.Application.Domain.Models;
using FirstSparrow.Application.Extensions;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
using FirstSparrow.Application.Shared;
using FirstSparrow.Infrastructure.Services.Ethereum.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

namespace FirstSparrow.Infrastructure.Services.Ethereum;

public class EthereumService : IBlockChainService
{
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<EthereumService> _logger;
    private readonly Web3 _web3;
    private readonly Contract _smartContract;
    private readonly IOptions<FirstSparrowConfigs> _firstSparrowConfigs;

    public EthereumService(
        IOptions<FirstSparrowConfigs> firstSparrowConfigs,
        IHttpClientFactory httpClientFactory,
        TimeProvider timeProvider,
        ILogger<EthereumService> logger)
    {
        _timeProvider = timeProvider;
        _logger = logger;
        _web3 = CreateWeb3Client(firstSparrowConfigs, httpClientFactory);
        _smartContract = _web3.Eth.GetContract(EthereumServiceConstants.DepositEventABI, firstSparrowConfigs.Value.SmartContractAddress);
        _firstSparrowConfigs = firstSparrowConfigs;
    }

    public async Task<(List<Deposit> deposits, ulong lastBlockChecked)> FetchDeposits(FetchDepositsParams @params, CancellationToken cancellationToken = default)
    {
        (IEnumerable<DepositEvent> events, ulong lastBlockChecked) = await FetchDepositEvents(@params, cancellationToken);
        return (events.Select(@event => new Deposit()
        {
            Commitment = @event.Commitment.ToHexForBytes32(),
            CreateTimestamp = _timeProvider.GetUtcNowDateTime(),
            UpdateTimestamp = null,
            DepositTimestamp = DateTimeOffset.FromUnixTimeSeconds(@event.Timestamp).UtcDateTime,
            Index = @event.LeafIndex,
            IsDeleted = false,
        }).ToList(), lastBlockChecked);
    }

    public async Task<BigInteger> HashConcat(BigInteger left, BigInteger right)
    {
        PoseidonFunction poseidonFunction = new PoseidonFunction
        {
            Input = [ left, right ]
        };

        IContractQueryHandler<PoseidonFunction> handler = _web3.Eth.GetContractQueryHandler<PoseidonFunction>();
        PoseidonFunctionOutput outPut = await handler.QueryAsync<PoseidonFunctionOutput>(_firstSparrowConfigs.Value.HasherSmartContractAddress, poseidonFunction);

        return outPut.Result;
    }

    private async Task<(IEnumerable<DepositEvent> events, ulong lastBlockChecked)> FetchDepositEvents(FetchDepositsParams @params, CancellationToken cancellationToken = default)
    {
        Event depositEvent = _smartContract.GetEvent(DepositEventConstants.EventName);
        (NewFilterInput filterInput, ulong lastBlockChecked)= await GenerateFilter(@params, depositEvent, cancellationToken);

        List<EventLog<DepositEvent>> eventLogs = await depositEvent.GetAllChangesAsync<DepositEvent>(filterInput);
        return (eventLogs.Select(eventLog => eventLog.Event), lastBlockChecked);
    }

    private async Task<(NewFilterInput filter, ulong lastCheckedBlock)> GenerateFilter(FetchDepositsParams @params, Event @event, CancellationToken cancellationToken)
    {
        ulong latestBlock = await GetLatestBlockNumber(cancellationToken);

        ulong toBlock = Math.Min(@params.FromBlock + (ulong)@params.BatchSize, latestBlock);

        return (@event.CreateFilterInput(
            fromBlock: new BlockParameter(@params.FromBlock),
            toBlock: new BlockParameter(toBlock)), toBlock);
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