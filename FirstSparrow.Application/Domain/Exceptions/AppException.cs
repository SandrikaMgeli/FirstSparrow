namespace FirstSparrow.Application.Domain.Exceptions;

public class AppException : Exception
{
    public readonly ExceptionCode ExceptionCode;

    public string? AppMessage { get; private set; }

    public AppException(string message, ExceptionCode code = ExceptionCode.GENERAL_ERROR) : base(message)
    {
        ExceptionCode = code;
        AppMessage = message;
    }

    public AppException(Exception ex, ExceptionCode code = ExceptionCode.GENERAL_ERROR) : base(ex.Message, ex)
    {
        ExceptionCode = code;
        AppMessage = null;
    }
}