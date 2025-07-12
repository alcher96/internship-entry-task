using FluentValidation;
using TicTacToe.API.DTOs;

namespace TicTacToe.API.Validators;

public class MoveDtoValidator : AbstractValidator<MoveDto>
{
    public MoveDtoValidator(int boardSize)
    {
        RuleFor(m => m.Player)
            .NotEmpty()
            .Must(p => p == "X" || p == "O")
            .WithMessage("Player must be 'X' or 'O' or the keyboard layout should be english");

        RuleFor(m => m.Position)
            .NotNull()
            .WithMessage("Position cannot be null.");

        RuleFor(m => m.Position.Row)
            .GreaterThanOrEqualTo(0)
            .LessThan(boardSize)
            .WithMessage($"Row must be between 0 and {boardSize - 1}.");

        RuleFor(m => m.Position.Column)
            .GreaterThanOrEqualTo(0)
            .LessThan(boardSize)
            .WithMessage($"Col must be between 0 and {boardSize - 1}.");
    }
}