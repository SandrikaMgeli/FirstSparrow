using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace FirstSparrow.Application.Shared;

public class FirstSparrowConfigs
{
    public BigInteger DenominationValue;

    [Required]
    public ulong InitialBlockIndex { get; set; }

    [Required]
    public string Denomination
    {
        get => DenominationValue.ToString();
        set => DenominationValue = BigInteger.Parse(value);
    }

    [Required]
    public string SmartContractAddress { get; set; }

    [Required]
    public string RpcUrl { get; set; }

    [Required]
    public string HasherSmartContractAddress { get; set; }
}