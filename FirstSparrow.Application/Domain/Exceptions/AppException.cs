namespace FirstSparrow.Application.Domain.Exceptions;

public class AppException : Exception
{
    public readonly ExceptionCode ExceptionCode;

    public string? AppMessage { get; private set; }

    public AppException(string message, ExceptionCode code) : base(message)
    {
        ExceptionCode = code;
        AppMessage = message;
    }

    public AppException(Exception ex, ExceptionCode code) : base(ex.Message, ex)
    {
        ExceptionCode = code;
        AppMessage = null;
    }

    public AppException(Exception ex, string message, ExceptionCode code) : base(ex.Message, ex)
    {
        ExceptionCode = code;
        AppMessage = message;
    }
}