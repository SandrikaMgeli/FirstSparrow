using System.Numerics;

namespace FirstSparrow.Application.Extensions;

public static class BigIntegerExtensions
{
    public static string ToHex(this BigInteger value, bool includePrefix = true)
    {
        string hex = value.ToString("X");
        return includePrefix ? "0x" + hex : hex;
    }
}