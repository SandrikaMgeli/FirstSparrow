namespace FirstSparrow.Application.Services.Abstractions;

public interface IOtpService
{
    int Generate(int numDigits);
}