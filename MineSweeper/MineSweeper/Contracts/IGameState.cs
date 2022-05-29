using MineSweeper.Player;

namespace MineSweeper.Contracts;

public interface IGameState
{
    bool IsInitialized { get; }

    (int column, int row) GetColumRows();

    int GetNumberOfTotalMines();

    int[]? GetBoard();

    int GetScore(int playerIndex);

    void Set(PlayContext context, int playerIndex);

    bool IsGameOver();
}
