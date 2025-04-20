using FirstSparrow.Application.Domain.Entities.Base;
using FirstSparrow.Application.Domain.Enums;

namespace FirstSparrow.Application.Domain.Entities;

public class Otp : BaseEntity<int>
{
    public int Code { get; set; }

    public string Destination { get; set; }

    public bool IsUsed { get; set; }

    public OtpUsage Usage { get; set; }

    public bool IsSent { get; set; }

    public DateTime ExpiresAt { get; set; }
}