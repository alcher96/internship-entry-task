using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.API;
using TicTacToe.API.DTOs;
using TicTacToe.Domain.Models;
using TicTacToe.Infrastructure;

namespace TicTaсToe.Tests;

public class MovesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly string _dbPath;

    public MovesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"TicTacToeTestDb_{Guid.NewGuid()}.db");

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite($"Data Source={_dbPath}"));
            });
        });
        _client = _factory.CreateClient();
    }
    
     

     
    [Fact]
    public async Task MakeMove_ValidMove_ReturnsOkWithGameState()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var move = new MoveDto { Player = "X", Position = new PositionDto { Row = 0, Column = 0 } };

        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var game = new Game
        {
            Id = gameId,
            CurrentPlayer = "X",
            Board = new string[3][] { new string[3], new string[3], new string[3] },
            MoveCount = 0
        };
        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await _client.PostAsJsonAsync($"/games/{gameId}/moves", move);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var gameState = await response.Content.ReadFromJsonAsync<GameStateDto>();
        Assert.NotNull(gameState);
        Assert.Equal(gameId, gameState.GameId);
        Assert.Equal("O", gameState.CurrentPlayer);
        Assert.Equal("X", gameState.Board[0][0]);
        Assert.Equal("ongoing", gameState.Status);

        using var scope2 = _factory.Services.CreateScope();
        var dbContext2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
        var updatedGame = await dbContext2.Games.FindAsync(gameId);
        Assert.NotNull(updatedGame);
        Assert.Equal("X", updatedGame.Board[0][0]);
        Assert.Equal("O", updatedGame.CurrentPlayer);
        
    }
       

    

        [Fact]
        public async Task MakeMove_GameNotFound_ReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var move = new MoveDto { Player = "X", Position = new PositionDto { Row = 0, Column = 0 } };

            // Act
            var response = await _client.PostAsJsonAsync($"/games/{gameId}/moves", move);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            var errors = await response.Content.ReadFromJsonAsync<ValidationErrorDto[]>();
            Assert.NotNull(errors);
            Assert.Single(errors);
            Assert.Equal("GameId", errors[0].PropertyName);
            Assert.Equal("Game not found.", errors[0].ErrorMessage);
            
        }
        
        //При гонке двух POST /moves с одинаковым телом второй запрос обязан вернуть 200 OK и тот же ETag
        //Жесткая конкурентность за счет IsolationLevel.Serializable транзакции
         [Fact]
        public async Task MakeMove_IdempotentConcurrentRequests_ReturnsSameETag()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var move = new MoveDto { Player = "X", Position = new PositionDto { Row = 0, Column = 0 } };

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
            var game = new Game
            {
                Id = gameId,
                CurrentPlayer = "X",
                Board = new string[3][] { new string[3], new string[3], new string[3] },
                MoveCount = 0
            };
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            // Act: Отправляем два одновременных запроса
            var task1 = _client.PostAsJsonAsync($"/games/{gameId}/moves", move);
            var task2 = _client.PostAsJsonAsync($"/games/{gameId}/moves", move);
            var responses = await Task.WhenAll(task1, task2);

            var response1 = responses[0];
            var response2 = responses[1];

            // Assert
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
            Assert.Equal(HttpStatusCode.OK, response2.StatusCode);

            var etag1 = response1.Headers.ETag?.Tag;
            var etag2 = response2.Headers.ETag?.Tag;
            Assert.NotNull(etag1);
            Assert.Equal(etag1, etag2); // Marvin.Cache.Headers обеспечивает одинаковый ETag для одинакового ответа

            var gameState1 = await response1.Content.ReadFromJsonAsync<GameStateDto>();
            var gameState2 = await response2.Content.ReadFromJsonAsync<GameStateDto>();
            Assert.NotNull(gameState1);
            Assert.NotNull(gameState2);
            Assert.Equal(gameId, gameState1.GameId);
            Assert.Equal("O", gameState1.CurrentPlayer);
            //Assert.Equal(1, gameState1.MoveCount);
            Assert.Equal("X", gameState1.Board[0][0]);
            Assert.Equal("ongoing", gameState1.Status);
            Assert.Equal(gameState1.GameId, gameState2.GameId);
            Assert.Equal(gameState1.CurrentPlayer, gameState2.CurrentPlayer);
            //Assert.Equal(gameState1.MoveCount, gameState2.MoveCount);
            Assert.Equal(gameState1.Board[0][0], gameState2.Board[0][0]);
            Assert.Equal(gameState1.Status, gameState2.Status);

            using var scope2 = _factory.Services.CreateScope();
            var dbContext2 = scope2.ServiceProvider.GetRequiredService<IDbContext>();
            var moves = await dbContext2.Moves
                .Where(m => m.GameId == gameId && m.Player == move.Player && m.Row == move.Position.Row && m.Col == move.Position.Column)
                .ToListAsync();
            Assert.Single(moves); // Проверяем, что ход добавлен только один раз

            
        }

       
}