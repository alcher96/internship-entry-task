using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.Contracts;
using TicTacToe.Domain.Models;

namespace TicTacToe.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly IDbContext _context;


    public GameRepository(IDbContext context)
    {
        _context = context;
    }
    
    public async Task<Game> GetGameByIdAsync(Guid gameId)
    {
        return await _context.Games
            .Include(g => g.Moves)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task AddAsync(Game game)
    {
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
    }
    
    public async Task<Move> GetMoveAsync(Guid gameId, string player, int row, int col)
    {
        return await _context.Moves
            .FirstOrDefaultAsync(m => m.GameId == gameId && m.Player == player && m.Row == row && m.Col == col);
    }

    public async Task AddMoveAsync(Move move)
    {
        await _context.Moves.AddAsync(move);
        await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
    {
        await _context.BeginTransactionAsync(isolationLevel);
    }

    public async Task CommitTransactionAsync()
    {
        await _context.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await _context.RollbackTransactionAsync();
    }
    
}