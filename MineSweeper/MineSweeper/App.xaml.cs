using Microsoft.Extensions.DependencyInjection;
using MineSweeper.Defines.Games;
using MineSweeper.Defines.Utils;
using MineSweeper.Services;
using MineSweeper.Utils;
using MineSweeper.Utils.Players;
using MineSweeper.ViewModels;
using System;
using System.Windows;

namespace MineSweeper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _services;

    public App()
    {
        _services = ConfigureServices();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // test

        var appViewModel = _services.GetService<AppViewModel>();
        var mainWindow = new MainWindow();
        mainWindow.DataContext = appViewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var consoleOut = _services.GetService<IConsoleOut>();
        consoleOut?.CloseConsole();

        base.OnExit(e);
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(Commons.Utils.Logger.GetLogger());
        services.AddSingleton<IConsoleOut, ConsoleOutRedirector>();
        services.AddSingleton<IDispatcherService, DispatcherService>();

        services.AddSingleton<INotificationPopup, NotificationPopupViewModel>();
        services.AddSingleton<IPlayerLoader, PlayerLoader>();
        services.AddSingleton<IGameState, GameViewModel>();
        services.AddSingleton<ITurnProcess, TurnPlayViewModel>();
        services.AddSingleton<AppViewModel>();

        return services.BuildServiceProvider();
    }
}
