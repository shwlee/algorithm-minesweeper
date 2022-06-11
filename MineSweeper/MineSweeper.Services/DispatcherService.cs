using MineSweeper.Defines.Utils;
using System.Windows;

namespace MineSweeper.Services;

public class DispatcherService : IDispatcherService
{
    public async Task BeginInvoke(Action action) => await Application.Current.Dispatcher.BeginInvoke(action);

    public bool CheckAccess()
    {
        return Application.Current.Dispatcher.CheckAccess();
    }

    public void Invoke(Action action)
    {
        Application.Current.Dispatcher.Invoke(action);
    }
}
