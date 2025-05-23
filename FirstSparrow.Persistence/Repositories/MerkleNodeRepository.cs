using System.Numerics;
using Dapper;
using FirstSparrow.Application.Domain.Entities;
using FirstSparrow.Application.Domain.Extensions;
using FirstSparrow.Application.Extensions;
using FirstSparrow.Application.Repositories.Abstractions;
using FirstSparrow.Persistence.Repositories.Base;

namespace FirstSparrow.Persistence.Repositories;

public class MerkleNodeRepository(
    DbManagementContext context) : BaseRepository<MerkleNode, int>(context, MerkleNodeRepositorySql.Insert, MerkleNodeRepositorySql.Delete, MerkleNodeRepositorySql.Update, MerkleNodeRepositorySql.GetById), IMerkleNodeRepository
{
    private readonly BigInteger[] _zeros =
    [
        BigInteger.Parse("21663839004416932945382355908790599225266501822907911457504978515578255421292"),
        BigInteger.Parse("8995896153219992062710898675021891003404871425075198597897889079729967997688"),
        BigInteger.Parse("15126246733515326086631621937388047923581111613947275249184377560170833782629"),
        BigInteger.Parse("6404200169958188928270149728908101781856690902670925316782889389790091378414"),
        BigInteger.Parse("17903822129909817717122288064678017104411031693253675943446999432073303897479"),
        BigInteger.Parse("11423673436710698439362231088473903829893023095386581732682931796661338615804"),
        BigInteger.Parse("10494842461667482273766668782207799332467432901404302674544629280016211342367"),
        BigInteger.Parse("17400501067905286947724900644309270241576392716005448085614420258732805558809"),
        BigInteger.Parse("7924095784194248701091699324325620647610183513781643345297447650838438175245"),
        BigInteger.Parse("3170907381568164996048434627595073437765146540390351066869729445199396390350"),
        BigInteger.Parse("21224698076141654110749227566074000819685780865045032659353546489395159395031"),
        BigInteger.Parse("18113275293366123216771546175954550524914431153457717566389477633419482708807"),
        BigInteger.Parse("1952712013602708178570747052202251655221844679392349715649271315658568301659"),
        BigInteger.Parse("18071586466641072671725723167170872238457150900980957071031663421538421560166"),
        BigInteger.Parse("9993139859464142980356243228522899168680191731482953959604385644693217291503"),
        BigInteger.Parse("14825089209834329031146290681677780462512538924857394026404638992248153156554"),
        BigInteger.Parse("4227387664466178643628175945231814400524887119677268757709033164980107894508"),
        BigInteger.Parse("177945332589823419436506514313470826662740485666603469953512016396504401819"),
        BigInteger.Parse("4236715569920417171293504597566056255435509785944924295068274306682611080863"),
        BigInteger.Parse("8055374341341620501424923482910636721817757020788836089492629714380498049891"),
    ];

    public async Task<MerkleNode?> GetByNodeCoordinate(long index, int layer, bool ensureExists, CancellationToken cancellationToken = default)
    {
        EnsureConnection();

        MerkleNode? merkleNode = await context.Connection!.QuerySingleOrDefaultAsync<MerkleNode>(
            MerkleNodeRepositorySql.GetByNodeCoordinate,
            new
            {
                Index = index,
                Layer = layer,
            }, context.Transaction);

        if(ensureExists)
        {
            merkleNode.EnsureExists($"Index = {index} and  Layer = {layer}");
        }

        return merkleNode;
    }

    public async Task<MerkleNode?> GetByCommitment(string commitment, bool ensureExists, CancellationToken cancellationToken = default)
    {
        EnsureConnection();

        MerkleNode? merkleNode = await context.Connection!.QueryFirstOrDefaultAsync<MerkleNode>(
            MerkleNodeRepositorySql.GetByCommitment,
            new
            {
                Commitment = commitment,
            }, context.Transaction);

        if(ensureExists)
        {
            merkleNode.EnsureExists($"commitment = {commitment}");
        }

        return merkleNode;
    }

    public async Task<MerkleNode> GetNeighbour(MerkleNode merkleNode, CancellationToken cancellationToken = default)
    {
        EnsureConnection();

        (long neighbourIndex, int neighbourLayer) = merkleNode.CalculateNeighbourCoordinates();

        MerkleNode? neighbourNode = await GetByNodeCoordinate(neighbourIndex, neighbourLayer, false, cancellationToken);

        if (neighbourNode is null)
        {
            return new MerkleNode(neighbourLayer)
            {
                Index = neighbourIndex,
                Commitment = GetZeroValue(neighbourLayer).ToHexForBytes32(),
            };
        }

        return neighbourNode;
    }

    public BigInteger GetZeroValue(int layer)
    {
        return _zeros[layer];
    }
}

public static class MerkleNodeRepositorySql
{
    public const string Insert = @$"
                                    INSERT INTO merkle_nodes
                                    (
                                        commitment,
                                        index,
                                        layer,
                                        deposit_timestamp,
                                        create_timestamp,
                                        update_timestamp,
                                        is_deleted
                                    )
                                    VALUES
                                    (
                                        @{nameof(MerkleNode.Commitment)},
                                        @{nameof(MerkleNode.Index)},
                                        @{nameof(MerkleNode.Layer)},
                                        @{nameof(MerkleNode.DepositTimestamp)},
                                        @{nameof(MerkleNode.CreateTimestamp)},
                                        @{nameof(MerkleNode.UpdateTimestamp)},
                                        @{nameof(MerkleNode.IsDeleted)}
                                    )
                                    RETURNING id;";

    public const string GetById = $@"
                                    SELECT
                                        id as {nameof(MerkleNode.Id)},
                                        commitment as {nameof(MerkleNode.Commitment)},
                                        index as {nameof(MerkleNode.Index)},
                                        layer as {nameof(MerkleNode.Layer)},
                                        deposit_timestamp as {nameof(MerkleNode.DepositTimestamp)},
                                        create_timestamp as {nameof(MerkleNode.CreateTimestamp)},
                                        update_timestamp as {nameof(MerkleNode.UpdateTimestamp)},
                                        is_deleted as {nameof(MerkleNode.IsDeleted)}
                                    from merkle_nodes where id = @{nameof(MerkleNode.Id)} and is_deleted = FALSE LIMIT 1;";

    public const string GetByNodeCoordinate = $@"
                                    SELECT
                                        id as {nameof(MerkleNode.Id)},
                                        commitment as {nameof(MerkleNode.Commitment)},
                                        index as {nameof(MerkleNode.Index)},
                                        layer as {nameof(MerkleNode.Layer)},
                                        deposit_timestamp as {nameof(MerkleNode.DepositTimestamp)},
                                        create_timestamp as {nameof(MerkleNode.CreateTimestamp)},
                                        update_timestamp as {nameof(MerkleNode.UpdateTimestamp)},
                                        is_deleted as {nameof(MerkleNode.IsDeleted)}
                                    from merkle_nodes 
                                    where 
                                        index = @{nameof(MerkleNode.Index)} and
                                        layer = @{nameof(MerkleNode.Layer)} and
                                        is_deleted = FALSE
                                    LIMIT 1;";

    public const string GetByCommitment = $@"
                                    SELECT
                                        id as {nameof(MerkleNode.Id)},
                                        commitment as {nameof(MerkleNode.Commitment)},
                                        index as {nameof(MerkleNode.Index)},
                                        layer as {nameof(MerkleNode.Layer)},
                                        deposit_timestamp as {nameof(MerkleNode.DepositTimestamp)},
                                        create_timestamp as {nameof(MerkleNode.CreateTimestamp)},
                                        update_timestamp as {nameof(MerkleNode.UpdateTimestamp)},
                                        is_deleted as {nameof(MerkleNode.IsDeleted)}
                                    from merkle_nodes 
                                    where 
                                        commitment = @{nameof(MerkleNode.Commitment)} and
                                        is_deleted = FALSE
                                    LIMIT 1";

    public const string Update = @$"
                                    UPDATE merkle_nodes
                                        SET
                                            commitment = @{nameof(MerkleNode.Commitment)},
                                            index = @{nameof(MerkleNode.Index)},
                                            layer = @{nameof(MerkleNode.Layer)},
                                            deposit_timestamp = @{nameof(MerkleNode.DepositTimestamp)},
                                            create_timestamp = @{nameof(MerkleNode.CreateTimestamp)},
                                            update_timestamp = @{nameof(MerkleNode.UpdateTimestamp)},
                                            is_deleted = @{nameof(MerkleNode.IsDeleted)}
                                        WHERE id = @{nameof(MerkleNode.Id)};";

    public const string Delete = $@"
                                    UPDATE merkle_nodes
                                        SET
                                            is_deleted = TRUE
                                        WHERE id = @{nameof(MerkleNode.Id)};";
}