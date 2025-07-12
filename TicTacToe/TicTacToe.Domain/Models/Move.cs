using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicTacToe.Domain.Models;

public class Move
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid GameId { get; set; }
    public string Player { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }

    [ForeignKey("GameId")]
    public Game Game { get; set; }
}