namespace MineSweeper.Models.Messages;

public class OpenBoxMessage
{
    public Box Opened { get; }

    public OpenBoxMessage(Box opened) => Opened = opened;
}
