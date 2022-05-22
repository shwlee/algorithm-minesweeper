using MineSweeper.Player;
using System.Collections.Generic;

namespace MineSweeper.Contracts;

public interface IPlayerLoader
{
    IEnumerable<IPlayer> LoadPlayers();
}
