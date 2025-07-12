using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TicTacToe.API.DTOs;
using TicTacToe.API.Validators;
using TicTacToe.API.Validators.Services;
using TicTacToe.Application.Contracts;
using TicTacToe.Application.Services;
using TicTacToe.Application.Validators;
using TicTacToe.Domain;
using TicTacToe.Infrastructure;
using TicTacToe.Infrastructure.Repositories;

namespace TicTacToe.API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices (this IServiceCollection services, IConfiguration configuration)
    {
         var connectionString = configuration.GetValue<string>("DefaultConnection") ?? "Data Source=game.db";

            // Регистрация DbContext
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
            services.AddScoped<IDbContext>(provider => provider.GetService<AppDbContext>());

            // Регистрация сервисов
            services.AddScoped<IGameRepository, GameRepository>();
            services.AddScoped<IGameService,GameService>();
            services.AddScoped<IGameLogic>(provider => new GameLogic(GetBoardSize(provider)));
           
            services.AddScoped<GameMapper>();

            // Регистрация валидаторов
            services.AddScoped<IMoveValidatorService, MoveValidatorService>();
            services.AddScoped<IValidator<Domain.Models.Game>>(provider => new GameValidator(GetBoardSize(provider)));
            services.AddScoped<IValidator<MoveDto>>(provider => new MoveDtoValidator(GetBoardSize(provider)));
        
            
            
    }
    private static int GetBoardSize(IServiceProvider provider)
    {
        var config = provider.GetService<IConfiguration>();
        if (config == null)
            throw new InvalidOperationException("Configuration is not available.");
        var boardSizeString = config["BOARD_SIZE"] ?? "3";
        if (!int.TryParse(boardSizeString, out var boardSize))
            throw new InvalidOperationException("BOARD_SIZE must be a valid integer.");
        return boardSize;
    }
}