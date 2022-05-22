using System;
using System.Runtime.Serialization;

namespace MineSweeper.Exceptions;


[Serializable]
public class GameOverException : Exception
{
    public int? GameOverPlayer { get; }
    public GameOverException(int? gameOverPlayer) : this(gameOverPlayer, null)
    {
    }

    public GameOverException(int? gameOverPlayer, string? message) : this(gameOverPlayer, message, null)
    {
    }

    public GameOverException(int? gameOverPlayer, string? message, Exception? inner) : base(message, inner)
    {
        GameOverPlayer = gameOverPlayer;
    }
    protected GameOverException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}
