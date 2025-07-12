using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TicTacToe.Domain.Models;

namespace TicTacToe.Infrastructure;

public class AppDbContext : DbContext , IDbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Move> Moves { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
      
    }
    
    public async Task BeginTransactionAsync(System.Data.IsolationLevel isolationLevel)
    {
        await Database.BeginTransactionAsync(isolationLevel);
    }

    public async Task CommitTransactionAsync()
    {
        await Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await Database.RollbackTransactionAsync();
    }
    
    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
    
}