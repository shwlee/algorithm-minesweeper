using System;

namespace MineSweeper.Extensions;

public static class console
{
#pragma warning disable IDE1006 // Naming Styles
    public static void log(object arg) => Console.WriteLine(arg);
#pragma warning restore IDE1006 // Naming Styles
}
