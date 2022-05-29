using System;
using System.Runtime.Serialization;

namespace MineSweeper.Exceptions;

[Serializable]
public class MineSweepViolationException : Exception
{
    public int? Player { get; }

    public ViolationReason Reason { get; }

    public MineSweepViolationException(int? player, ViolationReason reason) : this(player, reason, null)
    {
    }

    public MineSweepViolationException(int? player, ViolationReason reason, string? message) : this(player, reason, message, null)
    {
    }

    public MineSweepViolationException(int? player, ViolationReason reason, string? message, Exception? inner) : base(message, inner)
    {
        Player = player;
        Reason = reason;
    }

    protected MineSweepViolationException(
      SerializationInfo info,
      StreamingContext context) : base(info, context) { }
}

public enum ViolationReason
{
    TryOpenAlreadyOpened,

    TryOpenAlreadyMarked,

    TryOpenOutOfRange,

    TryMarkAlreadyOpened,

    TryMarkOutOfRange,

    TurnTimeOver
}