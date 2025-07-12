using Microsoft.EntityFrameworkCore;
using TicTacToe.Domain.Models;

namespace TicTacToe.Infrastructure;

public interface IDbContext
{
    // обернем контекст базы, чтобы репозиторий не работал напрямую с контекстом бд
    DbSet<Game> Games { get; set; }
    DbSet<Move> Moves { get; set; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel);
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}