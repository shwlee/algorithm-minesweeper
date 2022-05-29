namespace MineSweeper.Player;

public class SamplePlayer : IPlayer
{
    // sample!

    private int _myNumber;

    private int _column;

    private int _row;

    private int _totalMineCount;

    private string _name;

    public SamplePlayer(string name)
    {
        _name = name;
    }

    public string GetName()
    {
        return _name;
    }

    public void Initialize(int myNumber, int column, int row, int totalMineCount)
    {
        _myNumber = myNumber;
        _column = column;
        _row = row;
        _totalMineCount = totalMineCount;
    }

    public PlayContext Turn(int[] board, int turnCount)
    {
        var action = PlayerAction.Mark;
        var unopeneds = new List<int>();
        for (var i = 0; i < board.Length; i++)
        {
            var box = board[i];
            if (box is -1)
            {
                unopeneds.Add(i);
            }
        }

        if (unopeneds.Count is 0)
        {
            action =  PlayerAction.Close;
            return new PlayContext(action, 0);
        }

        int position;

        do
        {
            var selectedIndex = new Random().Next(0, unopeneds.Count - 1);
            var unopened = unopeneds[selectedIndex];
            position = unopened;
            
            // double check.
            var inBoard = board[position];
            if (inBoard is not -1)
            {
                continue;
            }

            break;
        }
        while (true);

        return new PlayContext(action, position);
    }
}
