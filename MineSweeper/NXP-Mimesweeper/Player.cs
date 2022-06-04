using MineSweeper.Player;

namespace NXP.CSharp.MineSweeper;

public class Player : IPlayer
{
    // sample!

    private SamplePlayer _sample;

    public Player()
    {
        _sample = new SamplePlayer();
    }

    public string GetName()
    {
        return "NXP Greg4";
    }

    public void Initialize(int myNumber, int column, int row, int totalMineCount)
    {
        _sample.Initialize(myNumber, column, row, totalMineCount);        
    }

    public PlayContext Turn(int[] board, int turnCount)
    {
        return _sample.Turn(board, turnCount);        
    }
}
