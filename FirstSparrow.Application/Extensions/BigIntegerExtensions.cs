using System.Numerics;
using System.Text;

namespace FirstSparrow.Application.Extensions;

public static class BigIntegerExtensions
{
    public static string ToHexForBytes32(this BigInteger value)
    {
        string hex = value.ToString("X");

        return "0x" + ZeroPadding(64 - hex.Length) + hex;
    }

    private static string ZeroPadding(int amount)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < amount; i++)
        {
            builder.Append("0");
        }

        return builder.ToString();
    }
}