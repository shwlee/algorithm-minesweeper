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
            new SamplePlayer(),
            new SamplePlayer(),
            new SamplePlayer(),
            new SamplePlayer(),
        };

        return players;
    }
}
