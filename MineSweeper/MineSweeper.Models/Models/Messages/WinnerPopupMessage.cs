namespace MineSweeper.Models.Models.Messages;

public class WinnerPopupMessage
{
    public IEnumerable<TurnPlayer> Players { get; }

    public WinnerPopupMessage(IEnumerable<TurnPlayer> players)
    {
        Players = players;
    }
}
