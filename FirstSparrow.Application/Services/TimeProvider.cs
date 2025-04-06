using FirstSparrow.Application.Services.Abstractions;

namespace FirstSparrow.Application.Services;

public class TimeProvider : ITimeProvider
{
    public DateTime GetUtcNow() => DateTime.UtcNow;
}