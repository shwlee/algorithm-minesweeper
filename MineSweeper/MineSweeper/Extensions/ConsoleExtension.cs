using NLog;
using System;

namespace MineSweeper.Extensions;

public class ConsoleLoggerMediator
{
    private readonly ILogger _logger;

    public ConsoleLoggerMediator(ILogger logger) => _logger = logger;

#pragma warning disable IDE1006 // Naming Styles

    public void log(object arg) => _logger.Info(arg);

#pragma warning restore IDE1006 // Naming Styles
}
