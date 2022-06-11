namespace MineSweeper.Models.Messages;

public abstract class BoxMessage
{
    public Box Target { get; }

    protected BoxMessage(Box target) => Target = target;
}
