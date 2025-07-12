using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.API.DTOs;
using TicTacToe.API.Validators.Services;
using TicTacToe.Application.Contracts;
using TicTacToe.Application.ExceptionsHandling;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Models;

namespace TicTacToe.API.Controllers;


[ApiController]
[Route("games/{gameId}/moves")]
public class MovesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly GameMapper _gameMapper;
    private readonly IMoveValidatorService _moveValidatorService;
    
    public MovesController(IGameService gameService, GameMapper gameMapper, IValidator<MoveDto> moveValidator, IMoveValidatorService moveValidatorService)
    {
        _gameService = gameService;
        _gameMapper = gameMapper;
        _moveValidatorService = moveValidatorService;
    }

    /// <summary>
    ///эндпоинд для выполнения хода
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> MakeMove(Guid gameId, [FromBody] MoveDto move)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Invalid JSON", Detail = "Request body is malformed." });
        
        try
        {
            var position = await _moveValidatorService.ValidateAndMapMoveAsync(move);
            await _gameService.MakeMoveAsync(gameId, move.Player, position);
            var game = await _gameService.GetGameAsync(gameId);
            var dto = _gameMapper.ToDto(game);
            return Ok(dto);
        }
        //более детальная валидацию (через кастомный эксепшн) вместо простого 400 ответа ( 400 в загололвке есть)
        catch (BusinessValidationException ex)
        {
            var errors = ex.Errors.Select(e => new
            {
                e.PropertyName,
                e.ErrorMessage
            });
            return StatusCode(ex.StatusCode, errors);
        }
    }
}