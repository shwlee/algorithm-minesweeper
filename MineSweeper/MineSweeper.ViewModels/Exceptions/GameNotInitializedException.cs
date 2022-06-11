using System;
using System.Runtime.Serialization;

namespace MineSweeper.ViewModels.Exceptions;


[Serializable]
public class GameNotInitializedExceptionException : Exception
{
    public GameNotInitializedExceptionException() { }

    public GameNotInitializedExceptionException(string message) : base(message) { }

    public GameNotInitializedExceptionException(string message, Exception inner) : base(message, inner) { }

    protected GameNotInitializedExceptionException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}
