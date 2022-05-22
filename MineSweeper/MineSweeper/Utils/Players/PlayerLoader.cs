using MineSweeper.Contracts;
using MineSweeper.Player;
using System.Collections.Generic;

namespace MineSweeper.Utils.Players;

public class PlayerLoader : IPlayerLoader
{
    public IEnumerable<IPlayer> LoadPlayers()
    {
        // test
        var players = new List<IPlayer>
        {
            new SamplePlayer("test1"),
            new SamplePlayer("test2"),
            new SamplePlayer("test3"),
            new SamplePlayer("test4"),
        };

        return players;
    }
}
