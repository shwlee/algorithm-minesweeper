namespace MineSweeper.Defines.Utils;

public interface IConsoleOut
{
    void LoadConsole();

    void CloseConsole();

    bool IsConsoleOpened { get; }
}
