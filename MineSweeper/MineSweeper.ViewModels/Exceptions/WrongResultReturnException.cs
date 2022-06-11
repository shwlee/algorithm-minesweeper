using System;
using System.Runtime.Serialization;

namespace MineSweeper.ViewModels.Exceptions;


[Serializable]
public class WrongResultReturnExceptionException : Exception
{
    public int PlayerIndex { get; }

    public WrongResultReturnExceptionException(int playerIndex) : this(playerIndex, null)
    {
    }

    public WrongResultReturnExceptionException(int playerIndex, string? message) : this(playerIndex, message, null)
    {
    }

    public WrongResultReturnExceptionException(int playerIndex, string? message, Exception? inner) : base(message, inner)
    {
        PlayerIndex = playerIndex;
    }

    protected WrongResultReturnExceptionException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}
