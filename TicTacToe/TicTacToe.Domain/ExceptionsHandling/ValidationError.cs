namespace TicTacToe.Application.ExceptionsHandling;

public class ValidationError
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}