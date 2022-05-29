using System;
using System.Threading.Tasks;

namespace MineSweeper.Contracts;

public interface IDispatcherService
{
    bool CheckAccess();

    Task BeginInvoke(Action action);

    void Invoke(Action action);
}
