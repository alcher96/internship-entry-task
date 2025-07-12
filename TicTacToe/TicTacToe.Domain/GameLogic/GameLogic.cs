using TicTacToe.Application.ExceptionsHandling;
using TicTacToe.Domain.Models;

namespace TicTacToe.Domain;

public class GameLogic : IGameLogic
{
    private readonly int _boardSize;

    public GameLogic(int boardSize)
    {
        _boardSize = boardSize;
    }

    public Game CreateNewGame()
    {
        var board = new string[_boardSize][];
        for (int i = 0; i < _boardSize; i++)
        {
            board[i] = new string[_boardSize];
        }

        return new Game
        {
            Id = Guid.NewGuid(),
            Board = board,
            CurrentPlayer = "X",
            MoveCount = 0,
            Winner = null,
            IsDraw = false,
            Version = 1
        };
    }

    public void MakeMove(Game game, string player, Position position)
    {
        if (game == null)
            throw new ArgumentNullException(nameof(game));

        var errors = new List<ValidationError>();
        if (player != game.CurrentPlayer)
            errors.Add(new ValidationError { PropertyName = "Player", ErrorMessage = "Not your turn." });
        if (position.Row < 0 || position.Row >= _boardSize || position.Column < 0 || position.Column >= _boardSize)
            errors.Add(new ValidationError { PropertyName = "Position", ErrorMessage = "Position is out of bounds." });
        if (game.Board[position.Row][position.Column] != null)
            errors.Add(new ValidationError { PropertyName = "Position", ErrorMessage = "Position is already taken." });
        if (game.Winner != null || game.IsDraw)
            errors.Add(new ValidationError { PropertyName = "Game", ErrorMessage = "Game is already finished." });

        if (errors.Any())
        {
            throw new BusinessValidationException(errors, 400);
        }


        string symbol = player;
        // каждый третий ход игры существует вероятность 10%, что поставится значок противника.
        if (game.MoveCount % 3 == 2 && Random.Shared.NextDouble() < 0.1)
        {
            symbol = symbol == "X" ? "O" : "X";
        }

        game.Board[position.Row][position.Column] = symbol;
        game.MoveCount++;

        if (CheckWin(game.Board, symbol))
        {
            game.Winner = symbol;
        }
        else if (IsBoardFull(game.Board))
        {
            game.IsDraw = true;
        }
        else
        {
            game.CurrentPlayer = game.CurrentPlayer == "X" ? "O" : "X";
        }

        game.Version++;
    }
    
    private bool CheckWin(string[][] board, string symbol)
    {
        //проверка победы по строкам, столбцам, и диагоналям
        for (int i = 0; i < _boardSize; i++)
        {
            if (board[i].All(cell => cell == symbol))
                return true;
        }
        
        for (int i = 0; i < _boardSize; i++)
        {
            if (board.Select(row => row[i]).All(cell => cell == symbol))
                return true;
        }
        
        bool diag1 = true, diag2 = true;
        for (int i = 0; i < _boardSize; i++)
        {
            if (board[i][i] != symbol) diag1 = false;
            if (board[i][_boardSize - 1 - i] != symbol) diag2 = false;
        }
        return diag1 || diag2;
    }

    private bool IsBoardFull(string[][] board)
    {
        return board.All(row => row.All(cell => cell != null));
    }
    
}