using System.Data;
using TicTacToe.Application.Contracts;
using TicTacToe.Domain.Models;
using TicTacToe.Infrastructure;

namespace TicTaсToe.Tests;

public class MockGameRepository : IGameRepository
{
    private readonly IDbContext _dbContext;
  // Игнорируем транзакции для теста т.к. InMemory бд, их не поддерживает
    public MockGameRepository(IDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    public async Task<Game> GetGameByIdAsync(Guid gameId)
    {
        return await _dbContext.Games.FindAsync(gameId);
    }

    public async Task AddAsync(Game game)
    {
        await _dbContext.Games.AddAsync(game);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        _dbContext.Games.Update(game);
        await _dbContext.SaveChangesAsync();
    }

    public Task<Move> GetMoveAsync(Guid gameId, string player, int row, int col)
    {
        throw new NotImplementedException();
    }

    public Task AddMoveAsync(Move move)
    {
        throw new NotImplementedException();
    }

    public Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
    {
        return Task.CompletedTask; 
    }

    public Task CommitTransactionAsync()
    {
        return Task.CompletedTask; 
    }

    public Task RollbackTransactionAsync()
    {
        return Task.CompletedTask; 
    }
}