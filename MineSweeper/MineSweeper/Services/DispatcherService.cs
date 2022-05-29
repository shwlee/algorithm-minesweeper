using MineSweeper.Contracts;
using System;
using System.Threading.Tasks;

namespace MineSweeper.Services;

public class DispatcherService : IDispatcherService
{
    public async Task BeginInvoke(Action action)
    {
        await App.Current.Dispatcher.BeginInvoke(action);
    }

    public bool CheckAccess()
    {
        return App.Current.Dispatcher.CheckAccess();
    }

    public void Invoke(Action action)
    {
        App.Current.Dispatcher.Invoke(action);
    }
}
