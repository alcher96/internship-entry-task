using TicTacToe.API.DTOs;
using TicTacToe.Domain.Models;

namespace TicTacToe.API.Validators.Services;

public interface IMoveValidatorService
{
    Task<Position> ValidateAndMapMoveAsync(MoveDto move);
}