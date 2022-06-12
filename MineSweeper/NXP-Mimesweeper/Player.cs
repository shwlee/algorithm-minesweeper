using MineSweeper.Player;

namespace NXP.CSharp.MineSweeper;

public class Player : IPlayer
{
    // sample!

    public Player()
    {
    }

    public string GetName()
    {
        return $"Greg-{DateTime.UtcNow.Ticks.ToString().Substring(14, 4)}";
    }

    public void Initialize(int myNumber, int column, int row, int totalMineCount)
    {
    }

    public PlayContext Turn(int[] board, int turnCount)
    {
        var unopened = board.Where(b => b is -1).ToList();
        var randomIndex = new Random().Next(unopened.Count);

        return new PlayContext(PlayerAction.Open, randomIndex);
    }
}
