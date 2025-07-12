using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Application.Contracts;
using TicTacToe.Application.ExceptionsHandling;
using TicTacToe.Application.Services;
using TicTacToe.Application.Validators;
using TicTacToe.Domain;
using TicTacToe.Domain.Models;
using TicTacToe.Infrastructure;
using TicTacToe.Infrastructure.Repositories;

namespace TicTaсToe.Tests;

public class GameServiceTests
{
    private readonly IGameService _gameService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScope _scope;
    private readonly AppDbContext _dbContext;

    public GameServiceTests()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string> { { "BOARD_SIZE", "3" } })
            .Build();

        services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));
        services.AddScoped<IDbContext>(provider => provider.GetService<AppDbContext>());
        services.AddScoped<IGameRepository, MockGameRepository>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGameLogic>(provider => new GameLogic(3));
        services.AddScoped<IValidator<Game>>(provider => new GameValidator(3));

        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
        _dbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _gameService = _scope.ServiceProvider.GetRequiredService<IGameService>();

        if (_gameService == null)
            throw new InvalidOperationException("Failed to resolve IGameService");
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _scope.Dispose();
    }
    [Fact]
    public async Task CreateGameAsync_CreatesAndSavesGame()
    {
        // Act
        var game = await _gameService.CreateGameAsync();

        // Assert
        var savedGame = await _dbContext.Games.FindAsync(game.Id);

        Assert.NotNull(savedGame);
        Assert.Equal("X", savedGame.CurrentPlayer);
        Assert.Equal(0, savedGame.MoveCount);
        Assert.Null(savedGame.Winner);
        Assert.False(savedGame.IsDraw);
    }

     

       

        [Fact]
        public async Task MakeMoveAsync_GameNotFound_ThrowsBusinessValidationException()
        {
            // Arrange
            var invalidGameId = Guid.NewGuid();
            var position = new Position
            {
                Column = 0,
                Row = 0
            };

            // Act & Assert
            try
            {
                await _gameService.MakeMoveAsync(invalidGameId, "X", position);
                Assert.Fail("Expected BusinessValidationException was not thrown.");
            }
            catch (BusinessValidationException ex)
            {
                Assert.Equal(404, ex.StatusCode);
                Assert.Contains(ex.Errors, e => e.PropertyName == "GameId" && e.ErrorMessage == "Game not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected exception: {ex.GetType().Name}, Message: {ex.Message}");
                throw;
            }
        }
    
}