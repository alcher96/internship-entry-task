using FluentValidation;
using TicTacToe.Application.Contracts;
using TicTacToe.Application.ExceptionsHandling;
using TicTacToe.Domain;
using TicTacToe.Domain.Models;

namespace TicTacToe.Application.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameLogic _gameLogic;
    private readonly IValidator<Game> _validator;

    public GameService(IGameRepository gameRepository, IGameLogic gameLogic, IValidator<Game> validator)
    {
        _gameRepository = gameRepository;
        _gameLogic = gameLogic;
        _validator = validator;
    }

    public async Task<Game> CreateGameAsync()
    {
        var game = _gameLogic.CreateNewGame();
        await _validator.ValidateAndThrowAsync(game);
        await _gameRepository.AddAsync(game);
        return game;
    }

    public async Task<Game> GetGameAsync(Guid gameId)
    {
        var game = await _gameRepository.GetGameByIdAsync(gameId);
        if (game == null)
            throw new BusinessValidationException(new[] { new ValidationError { PropertyName = "GameId", ErrorMessage = "Game not found." } },400);
        await _validator.ValidateAndThrowAsync(game);
        return game;
    }

    public async Task MakeMoveAsync(Guid gameId, string player, Position position)
    {
        await _gameRepository.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        //жесткая конкурентность через транзакции
        try
        {
            var game = await _gameRepository.GetGameByIdAsync(gameId);
            if (game == null)
                throw new BusinessValidationException(new[] { new ValidationError { PropertyName = "GameId", ErrorMessage = "Game not found." } }, 404);
            
            var existingMove = await _gameRepository.GetMoveAsync(gameId, player, position.Row, position.Column);
            if (existingMove != null)
            {
                
                await _gameRepository.CommitTransactionAsync();
                return;
            }
            
            _gameLogic.MakeMove(game, player, position);
            await _validator.ValidateAndThrowAsync(game);
            var move = new Move
            {
                GameId = gameId,
                Player = player,
                Row = position.Row,
                Col = position.Column
            };
            await _gameRepository.AddMoveAsync(move);
            await _gameRepository.UpdateAsync(game);
            await _gameRepository.CommitTransactionAsync();
        }
        catch
        {
            await _gameRepository.RollbackTransactionAsync();
            throw;
        }
    }
}