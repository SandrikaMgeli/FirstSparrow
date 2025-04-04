namespace FirstSparrow.Application.Domain.Exceptions;

public class AppException : Exception
{
    public readonly ExceptionCode ExceptionCode;

    public AppException(string message, ExceptionCode code = ExceptionCode.GENERAL_ERROR) : base(message)
    {
        ExceptionCode = code;
    }

    public AppException(Exception ex, ExceptionCode code = ExceptionCode.GENERAL_ERROR) : base(ex.Message)
    {
        ExceptionCode = code;
    }
}