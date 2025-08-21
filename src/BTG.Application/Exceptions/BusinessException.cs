namespace BTG.Application.Exceptions;

public class BusinessException : Exception
{
    public int Status { get; }

    public BusinessException(string message, int status = 400) : base(message)
    {
        Status = status;
    }
}
