using NLog;

namespace MineSweeper.Commons.Utils;

public class Logger
{
    public static ILogger GetLogger()
    {
        var logger = LogManager.GetCurrentClassLogger();
        return logger;
    }
}
