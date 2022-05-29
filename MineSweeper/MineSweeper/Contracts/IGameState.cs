using MineSweeper.Player;

namespace MineSweeper.Contracts;

public interface IGameState
{
    bool IsInitialized { get; }

    (int column, int row) GetColumRows();

    int GetNumberOfTotalMines();

    int[]? GetBoard();

    void Set(PlayContext context, int playerIndex);

    bool IsGameOver();    
}
