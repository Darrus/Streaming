namespace Streaming.Exceptions;

public class AppException : Exception
{
    public ErrorCode error;
    public string ErrorMessage { 
        get {
            return $"{error.Code}: {error.Message}";
        }
    }
    
    public AppException(ErrorCode error) : base(error.Message)
    {
        this.error = error;
    }
}