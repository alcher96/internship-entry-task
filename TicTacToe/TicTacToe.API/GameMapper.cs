using TicTacToe.API.DTOs;
using TicTacToe.Domain.Models;

namespace TicTacToe.API;

public class GameMapper
{
    public virtual GameStateDto ToDto(Game game)
    {
        return new GameStateDto
        {
            GameId = game.Id,
            Board = game.Board,
            CurrentPlayer = game.CurrentPlayer,
            Status = game.Winner != null ? "won" : game.IsDraw ? "draw" : "ongoing",
            Winner = game.Winner,
            IsDraw = game.IsDraw
        };
    }
}