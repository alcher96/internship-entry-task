namespace TicTacToe.API.DTOs;

public class MoveDto
{
    public string Player { get; set; }
    public PositionDto Position { get; set; }
}