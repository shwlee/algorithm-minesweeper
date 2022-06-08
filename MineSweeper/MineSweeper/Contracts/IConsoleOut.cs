namespace MineSweeper.Contracts;

public interface IConsoleOut
{
    void LoadConsole();

    void CloseConsole();

    bool IsConsoleOpened { get; }
}
