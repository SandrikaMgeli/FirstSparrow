namespace FirstSparrow.Application.Extensions;

public static class TimeProviderExtensions
{
    public static DateTime GetUtcNowDateTime(this TimeProvider timeProvider)
    {
        return timeProvider.GetUtcNow().UtcDateTime;
    }
}