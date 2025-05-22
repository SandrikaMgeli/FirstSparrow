using System.Numerics;
using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Exceptions;

namespace FirstSparrow.Application.Domain.Entities;

public class MerkleNode : BaseEntity<int>
{
    private const int MerkleRootLayer = 20;

    public string Commitment { get; set; }

    public long Index { get; set; }

    public int Layer { get; private set; }

    public DateTime? DepositTimestamp { get; set; }

#pragma warning disable CS8618
    private MerkleNode() { } // This constructor is needed for ORM mapping

    public MerkleNode(int layer)
    {
        this.Layer = layer;
    }
#pragma warning restore CS8618

    public long CalculatePreviousDepositIndex()
    {
        EnsureNodeIsDeposit();

        if (Index == 0)
        {
            throw new AppException("Node is first one and doesn't have previous node", ExceptionCode.GENERAL_ERROR);
        }

        return Index - 1;
    }

    public (long neighbourIndex, int neighbourLayer) CalculateNeighbourCoordinates()
    {
        if (IsRoot)
        {
            throw new AppException("Node is root and doesn't have neighbour", ExceptionCode.GENERAL_ERROR);
        }

        long neighbourIndex = Index % 2 == 1 ? Index - 1 : Index + 1;
        int layer = Layer;

        return (neighbourIndex, layer);
    }
    

    public (long index, int layer) CalculateParentCoordinate()
    {
        return (Index / 2, Layer + 1);
    }

    public void EnsureNodeIsDeposit()
    {
        if (Layer != 0)
        {
            throw new AppException("Deposit's node layer must be zero", ExceptionCode.GENERAL_ERROR);
        }
    }

    public BigInteger GetCommitmentAsBigInteger()
    {
        if (Commitment is null)
        {
            throw new AppException("Commitment was null", ExceptionCode.GENERAL_ERROR);
        }

        BigInteger bigInt = BigInteger.Parse(Commitment.AsSpan(2), System.Globalization.NumberStyles.HexNumber);

        return bigInt;
    }

    public bool IsRoot => Layer == MerkleRootLayer && Index == 0;
}