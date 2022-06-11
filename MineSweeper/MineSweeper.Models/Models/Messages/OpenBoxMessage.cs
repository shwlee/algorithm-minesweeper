namespace MineSweeper.Models.Messages;

public class OpenBoxMessage : BoxMessage
{
    public OpenBoxMessage(Box opened) : base(opened)
    {
    }
}
