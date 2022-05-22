using MineSweeper.Player;

namespace MineSweeper.Contracts;

public interface IGameState
{
    bool IsInitialized { get; }

    (int column, int row) GetColumRows();

    int[]? GetBoard();

    void Set(PlayContext context, int playerIndex);
}
