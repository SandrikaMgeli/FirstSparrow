namespace FirstSparrow.Application.Services.Abstractions;

public interface ITimeProvider
{
    DateTime GetUtcNow();
}