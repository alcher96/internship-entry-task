using Microsoft.AspNetCore.Mvc;
using TicTacToe.Application.Services;
using TicTacToe.API.DTOs;
using TicTacToe.Application.Contracts;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("games")]
public class GameController :ControllerBase
{
    private readonly IGameService _gameService;
    private readonly GameMapper _gameMapper;

    public GameController(IGameService gameService, GameMapper gameMapper)
    {
        _gameService = gameService;
        _gameMapper = gameMapper;
    }

    /// <summary>
    /// Определяем эндпоинт для создания новой игры
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> CreateGame()
    {
        var game = await _gameService.CreateGameAsync();
        var dto = _gameMapper.ToDto(game);
        return Ok(dto);
    }
    /// <summary>
    /// Получаем игру по Id
    /// </summary>
    /// <returns></returns>
    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGame(Guid gameId)
    {
        var game = await _gameService.GetGameAsync(gameId);
        if (game == null) return NotFound();
        var dto = _gameMapper.ToDto(game);
        return Ok(dto);
    }
}