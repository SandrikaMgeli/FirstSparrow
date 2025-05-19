using System.Numerics;
using System.Text.Json;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Services.Models;
using FirstSparrow.Application.Shared;
using Microsoft.Extensions.Options;
using Nethereum.ABI.FunctionEncoding.Attributes;
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

    private static Web3 CreateWeb3Client(IOptions<FirstSparrowConfigs> firstSparrowConfigs, IHttpClientFactory httpClientFactory)
    {
        HttpClient client = httpClientFactory.CreateClient("ethereum_rpc");
        SimpleRpcClient rpcClient = new SimpleRpcClient(new Uri(firstSparrowConfigs.Value.RpcUrl), client);
        return new Web3(rpcClient);
    }

    public async Task<List<Deposit>> FetchDeposits(FetchDepositsParams @params, CancellationToken cancellationToken = default)
    {
        HexBigInteger latestBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync(cancellationToken);
        var depositEvent = _smartContract.GetEvent("Deposit");
        var filterInput = depositEvent.CreateFilterInput(
            fromBlock: new BlockParameter(@params.FromBlock),
            toBlock: new BlockParameter(@params.FromBlock + (ulong)@params.BatchSize)
        );
        var events = await depositEvent.GetAllChangesAsync<DepositEventDTO>(filterInput);
        foreach (var eventLog in events)
        {
            try
            {
                Console.WriteLine("0x" + Convert.ToHexString(eventLog.Event.Commitment).ToLower());
                Console.WriteLine(eventLog.Event.Timestamp);
                Console.WriteLine(eventLog.Event.LeafIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding event at block {eventLog.Log.BlockNumber}: {ex.Message}");
                // Continue processing other events
            }
        }
        return null;
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

[Event("Deposit")]
public class DepositEventDTO : IEventDTO
{
    [Parameter("bytes32", "commitment", 1, true)]
    public byte[] Commitment { get; set; }

    [Parameter("uint32", "leafIndex", 2, false)]
    public uint LeafIndex { get; set; }

    [Parameter("uint256", "timestamp", 3, false)]
    public BigInteger Timestamp { get; set; } //UTC unix timestamp
}