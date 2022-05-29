using System.Threading.Tasks;

namespace MineSweeper.Extensions;

public static class TaskExtension
{
    public static Task EnsureTask(this Task? task)
    {
        if (task is null)
        {
            return Task.CompletedTask;
        }

        return task;
    }
}
