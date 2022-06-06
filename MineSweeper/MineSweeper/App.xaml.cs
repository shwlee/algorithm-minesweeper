﻿using Microsoft.Extensions.DependencyInjection;
using MineSweeper.Contracts;
using MineSweeper.Services;
using MineSweeper.Utils.Players;
using MineSweeper.ViewModels;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace MineSweeper;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [DllImport("kernel32.dll")]
    public static extern Boolean AllocConsole();

    [DllImport("kernel32.dll")]
    public static extern Boolean FreeConsole();

    private IServiceProvider _services;

    public App()
    {
        _services = ConfigureServices();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        // test
#if DEBUG
        if (Debugger.IsAttached == false)
        {
            AllocConsole();
        }
#endif

        var appViewModel = _services.GetService<AppViewModel>();
        var mainWindow = new MainWindow();
        mainWindow.DataContext = appViewModel;
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        FreeConsole();

        base.OnExit(e);
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<AppViewModel>();
        services.AddSingleton<IDispatcherService, DispatcherService>();
        services.AddSingleton<IPlayerLoader, PlayerLoader>();
        services.AddSingleton<IGameState, GameViewModel>();
        services.AddSingleton<ITurnProcess, TurnPlayViewModel>();

        return services.BuildServiceProvider();
    }
}
