namespace MineSweeper.Player;

public class SamplePlayer : IPlayer
{
    // sample!

    private int _myNumber;

    private int _column;

    private int _row;

    public string GetName()
    {
        return "NXP Greg C# sample";
    }

    public void Initialize(int myNumber, int column, int row)
    {
        _myNumber = myNumber;
        _column = column;
        _row = row;
    }

    public PlayContext Turn(int[] board, int turnCount, int myScore)
    {
        var action = (PlayerAction)new Random().Next(0, 2);
        var position = new Random().Next(0, board.Length - 1);
        return new PlayContext(action, position);
    }
}
