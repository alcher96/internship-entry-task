using TicTacToe.Domain.Models;

namespace TicTacToe.API.DTOs;

public class GameStateDto
{
    public Guid GameId { get; set; }
    public string[][] Board { get; set; }
    public string CurrentPlayer { get; set; }
    public string Status { get; set; }
    public string Winner { get; set; }
    public bool IsDraw { get; set; }
    
 
}