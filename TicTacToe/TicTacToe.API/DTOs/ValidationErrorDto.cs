namespace TicTacToe.API.DTOs;

public class ValidationErrorDto
{
    public string PropertyName { get; set; }
    public string ErrorMessage { get; set; }
}