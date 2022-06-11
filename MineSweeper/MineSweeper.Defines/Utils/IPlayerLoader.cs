using MineSweeper.Player;

namespace MineSweeper.Defines.Utils;

public interface IPlayerLoader
{
    IEnumerable<IPlayer> LoadPlayers();
}