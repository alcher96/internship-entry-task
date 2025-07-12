using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.Contracts;
using TicTacToe.Application.Services;
using TicTacToe.Application.Validators;
using TicTacToe.Domain;
using TicTacToe.Infrastructure;
using TicTacToe.Infrastructure.Repositories;
using TicTacToe.API;
using FluentValidation;
using TicTacToe.Domain.Models;
using TicTacToe.API.DTOs;
using TicTacToe.API.Extensions;
using TicTacToe.API.Validators;


namespace TicTacToe.API
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.ConfigureServices(builder.Configuration);
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "Tic-Tac-Toe API", Version = "v1" });
            });
            
            builder.Services.AddOpenApi();
            
            //  Marvin.Cache.Headers для eTag
            builder.Services.AddHttpCacheHeaders(
                expirationModelOptions =>
                {
                    expirationModelOptions.MaxAge = 65; 
                    expirationModelOptions.SharedMaxAge = 65;
                },
                validationModelOptions =>
                {
                    validationModelOptions.MustRevalidate = true;
                });

            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tic-Tac-Toe API v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.Urls.Add("http://*:8080");
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
