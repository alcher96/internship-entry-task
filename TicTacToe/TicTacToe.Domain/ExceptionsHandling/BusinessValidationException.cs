namespace TicTacToe.Application.ExceptionsHandling;

public class BusinessValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; }
    public int StatusCode { get; }

    public BusinessValidationException(IEnumerable<ValidationError> errors, int statusCode = 400)
        : base("Business validation failed.")
    {
        Errors = errors ?? throw new ArgumentNullException(nameof(errors));
        StatusCode = statusCode;
    }
    
    public BusinessValidationException(IEnumerable<ValidationError> errors)
        : base("Business validation failed.")
    {
        Errors = errors;
    }
}