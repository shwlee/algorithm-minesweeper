using Microsoft.Win32.SafeHandles;
using MineSweeper.Contracts;
using System;
using System.IO;
using System.Text;

namespace MineSweeper.Utils;

public class ConsoleOutRedirector : IConsoleOut, IDisposable
{
    private SafeFileHandle? _safeFileHandle;

    public bool IsConsoleOpened { get; private set; }

    public ConsoleOutRedirector()
    {
    }

    public void LoadConsole()
    {
        NativeMethods.AllocConsole();

        InitConsole();

        IsConsoleOpened = true;
    }

    public void CloseConsole()
    {
        _safeFileHandle?.Close();
        _safeFileHandle = null;

        NativeMethods.FreeConsole();

        IsConsoleOpened = false;
    }

    private void InitConsole()
    {
        // Use AllocConsole instead of Visual Studio Output
        // https://stackoverflow.com/questions/41624103/console-out-output-is-showing-in-output-window-needed-in-allocconsole
        IntPtr stdHandle = NativeMethods.CreateFile(
                "CONOUT$",
                NativeMethods.GENERIC_WRITE,
                NativeMethods.FILE_SHARE_WRITE,
                0, NativeMethods.OPEN_EXISTING, 0, 0
            );

        _safeFileHandle = new SafeFileHandle(stdHandle, true);


        var fileStream = new FileStream(_safeFileHandle, FileAccess.Write);
        var encoding = new UTF8Encoding();
        var standardOutput = new StreamWriter(fileStream, encoding);
        standardOutput.AutoFlush = true;
        Console.SetOut(standardOutput);
    }

    public void Dispose()
    {
        CloseConsole();
    }
}
