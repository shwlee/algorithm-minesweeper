namespace MineSweeper.Models.Messages;

public class GameMessage
{
    public GameStateMessage State { get; }

    public GameMessage(GameStateMessage state) => State = state;
}
