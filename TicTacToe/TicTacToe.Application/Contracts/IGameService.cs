using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Contracts;

public interface IGameService
{
    Task<Game> CreateGameAsync();
    Task MakeMoveAsync(Guid gameId, string player, Position position);
    Task<Game> GetGameAsync(Guid gameId);
}