using TicTacToe.Application.ExceptionsHandling;
using TicTacToe.Domain;
using TicTacToe.Domain.Models;

namespace TicTaсToe.Tests;

public class GameLogicTests
{
    private readonly GameLogic _gameLogic;
    
    public GameLogicTests()
    {
        _gameLogic = new GameLogic(boardSize: 3);
    }

    [Fact]
    public void CreateNewGame_ReturnsValidGame()
    {
        // Act
        var game = _gameLogic.CreateNewGame();

        // Assert
        Assert.NotEqual(Guid.Empty, game.Id);
        Assert.Equal("X", game.CurrentPlayer);
        Assert.Equal(0, game.MoveCount);
        Assert.Null(game.Winner);
        Assert.False(game.IsDraw);
        Assert.Equal(3, game.Board.Length);
        Assert.All(game.Board, row => Assert.All(row, cell => Assert.Null(cell))); 
    }
    [Fact]
    public void MakeMove_ValidMove_UpdatesGameState()
    {
        // Arrange
        var game = _gameLogic.CreateNewGame();
        var position = new Position
        {
                Column = 0,
                Row = 0
        };
        

        // Act
        _gameLogic.MakeMove(game, "X", position);

        // Assert
        Assert.Equal("X", game.Board[0][0]);
        Assert.Equal("O", game.CurrentPlayer);
        Assert.Equal(1, game.MoveCount);
        Assert.Null(game.Winner);
        Assert.False(game.IsDraw);
    }

    [Fact]
    public void MakeMove_WrongPlayer_ThrowsBusinessValidationException()
    {
        // Arrange
        var game = _gameLogic.CreateNewGame(); // CurrentPlayer = "X"
        var position = new Position
        {
            Column = 0,
            Row = 0
        };

        // Act & Assert
        var exception = Assert.Throws<BusinessValidationException>(() =>
            _gameLogic.MakeMove(game, "O", position));
        Assert.Equal(400, exception.StatusCode);
        Assert.Contains(exception.Errors, e => e.PropertyName == "Player" && e.ErrorMessage == "Not your turn.");
    }
}