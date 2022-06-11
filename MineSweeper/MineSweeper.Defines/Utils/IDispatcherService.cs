namespace MineSweeper.Defines.Utils;

public interface IDispatcherService
{
    bool CheckAccess();

    Task BeginInvoke(Action action);

    void Invoke(Action action);
}
