using TicTacToe.Domain.Models;

namespace TicTacToe.Domain;

public interface IGameLogic
{
    Game CreateNewGame();
    void MakeMove(Game game, string player, Position position);
}