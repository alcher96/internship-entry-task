namespace TicTacToe.Domain.Models;

public class Game
{
    public Guid Id { get; set; }
    
    public string[][] Board { get; set; }
    
    public string CurrentPlayer { get; set; }
    
    public int MoveCount { get; set; }
    
    public string? Winner { get; set; }
    
    public bool IsDraw { get; set; }
    
    public int Version {get; set;}
    
    public List<Move> Moves { get; set; } = new List<Move>(); 
}