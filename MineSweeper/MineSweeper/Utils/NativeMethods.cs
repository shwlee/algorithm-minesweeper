using System;
using System.Runtime.InteropServices;

namespace MineSweeper.Utils;

public static class NativeMethods
{
    internal const uint GENERIC_WRITE = 0x40000000;
    internal const uint FILE_SHARE_WRITE = 0x2;
    internal const uint OPEN_EXISTING = 0x3;

    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        uint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        uint hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FreeConsole();
}
