namespace FirstSparrow.Application.Services.Abstractions;

public interface ICryptographyService
{
    string Hash(string text);
}