namespace MineSweeper.Player;

public class SamplePlayer : IPlayer
{
    // sample!

    private int _myNumber;

    private int _column;

    private int _row;

    private string _name;

    public SamplePlayer(string name)
    {
        _name = name;
    }

    public string GetName()
    {
        return _name;
    }

    public void Initialize(int myNumber, int column, int row)
    {
        _myNumber = myNumber;
        _column = column;
        _row = row;
    }

    public PlayContext Turn(int[] board, int turnCount)
    {
        var action = (PlayerAction)new Random().Next(0, 2);
        var position = new Random().Next(0, board.Length - 1);
        return new PlayContext(action, position);
    }
}
