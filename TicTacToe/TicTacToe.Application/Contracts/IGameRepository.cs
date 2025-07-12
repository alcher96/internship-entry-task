using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Contracts;

public interface IGameRepository
{
    Task<Game> GetGameByIdAsync(Guid gameId);
    Task AddAsync(Game game);
    Task UpdateAsync(Game game);
    Task<Move> GetMoveAsync(Guid gameId, string player, int row, int col);
    Task AddMoveAsync(Move move);
    Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel);
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}