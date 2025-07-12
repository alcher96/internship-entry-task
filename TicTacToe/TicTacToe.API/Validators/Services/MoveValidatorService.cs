using FluentValidation;
using TicTacToe.API.DTOs;
using TicTacToe.Application.ExceptionsHandling;
using TicTacToe.Domain.Models;

namespace TicTacToe.API.Validators.Services;

public class MoveValidatorService : IMoveValidatorService
{
    
    private readonly IValidator<MoveDto> _moveValidator;

    public MoveValidatorService(IValidator<MoveDto> moveValidator)
    {
        _moveValidator = moveValidator;
    }
    
    
    public async Task<Position> ValidateAndMapMoveAsync(MoveDto move)
    {
        var validationResult = await _moveValidator.ValidateAsync(move);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new ValidationError
            {
                PropertyName = e.PropertyName,
                ErrorMessage = e.ErrorMessage
            });
            throw new BusinessValidationException(errors, 400);
        }

        return new Position
        {
            Row = move.Position.Row,
            Column = move.Position.Column
        };
    }
}