namespace MineSweeper.Commons.Extensions;

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

    public static async Task<T> Timeout<T>(this Task<T> task, TimeSpan timeLimit)
    {
        var winner = await Task.WhenAny(task, DelayedDummyResultTask<T>(timeLimit));
        if (winner == task)
        {
            return await task;
        }

        throw new TimeoutException();
    }

    private static async Task<T> DelayedDummyResultTask<T>(TimeSpan delay)
    {
        await Task.Delay(delay);
        return default;
    }
}
