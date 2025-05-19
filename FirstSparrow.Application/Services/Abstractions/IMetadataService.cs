namespace FirstSparrow.Application.Services.Abstractions;

public interface IMetadataService
{
    Task InitializeNecessaryMetadata(CancellationToken cancellationToken = default);
}