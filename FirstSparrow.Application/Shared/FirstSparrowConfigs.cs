using System.ComponentModel.DataAnnotations;

namespace FirstSparrow.Application.Shared;

public class FirstSparrowConfigs
{
    [Required]
    public ulong InitialBlockIndex { get; set; }
}