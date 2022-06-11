namespace MineSweeper.Models;

public struct MinePosition
{
    public int X { get; set; }

    public int Y { get; set; }

    public MinePosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(MinePosition mine, MinePosition other)
    {
        return mine.X == other.X && mine.Y == other.Y;
    }

    public static bool operator !=(MinePosition mine, MinePosition other)
    {
        return mine.X != other.X || mine.Y != other.Y;
    }

    public override bool Equals(object? obj) =>  obj is MinePosition other && this.Equals(other);

    public bool Equals(MinePosition other) => this == other;

    public override int GetHashCode() => this.GetHashCode();
}
