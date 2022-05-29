using System;
using System.Runtime.Serialization;

namespace MineSweeper.Exceptions;


[Serializable]
public class TurnContinueException : Exception
{
    public TurnContinueException() { }

    public TurnContinueException(string message) : base(message) { }

    public TurnContinueException(string message, Exception inner) : base(message, inner) { }

    protected TurnContinueException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}
