using System.Runtime.InteropServices;

namespace MineSweeper.Commons.Utils;

public static class NativeMethods
{
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint FILE_SHARE_WRITE = 0x2;
    public const uint OPEN_EXISTING = 0x3;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        uint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        uint hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FreeConsole();
}
