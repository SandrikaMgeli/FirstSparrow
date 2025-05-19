using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Services.Abstractions;
using FirstSparrow.Application.Shared;
using Microsoft.Extensions.Options;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;

namespace FirstSparrow.Infrastructure.Services;

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

    public async Task<List<Deposit>> FetchDeposits(int batchMaxSize, CancellationToken cancellationToken = default)
    {
        HexBigInteger latestBlock = await _web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();

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