using FluentValidation;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Validators;

public class GameValidator : AbstractValidator<Game>
{
    public GameValidator(int boardSize)
    {
        RuleFor(g => g.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("Id cannot be empty");
        
        
        RuleFor(g => g.Board)
            .NotNull()
            .WithMessage("Board cannot be null.")
            .Must(b => b != null && b.Length == boardSize && b.All(row => row != null && row.Length == boardSize))
            .WithMessage($"Board must be a {boardSize}x{boardSize} array.")
            .ForEach(row => row
                .Must(cells => cells.All(cell => cell == null || cell == "X" || cell == "O"))
                .WithMessage("Board cells must be null, 'X', or 'O'."));

        RuleFor(g => g.CurrentPlayer)
            .NotEmpty()
            .Must(p => p == "X" || p == "O")
            .WithMessage("CurrentPlayer must be 'X' or 'O'.");

        RuleFor(g => g.MoveCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("MoveCount cannot be negative.");

        RuleFor(g => g.Winner)
            .Must(w => w == null || w == "X" || w == "O")
            .WithMessage("Winner must be null, 'X', or 'O'.");

        RuleFor(g => g.IsDraw)
            .NotNull()
            .WithMessage("IsDraw must be specified.");

        RuleFor(g => g.Version)
            .GreaterThan(0)
            .WithMessage("Version must be positive.");
    }
}