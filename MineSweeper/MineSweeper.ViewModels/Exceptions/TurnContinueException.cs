using System.Runtime.Serialization;

namespace MineSweeper.ViewModels.Exceptions;


[Serializable]
public class TurnContinueException : Exception
{
    public int? Player { get; }

    public TurnContinueException(int? player) : this(player, null) { }

    public TurnContinueException(int? player, string? message) : this(player, message, null) { }

    public TurnContinueException(int? player, string? message, Exception? inner) : base(message, inner)
    {
        Player = player;
    }

    protected TurnContinueException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}
