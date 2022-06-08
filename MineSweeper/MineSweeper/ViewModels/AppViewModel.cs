using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Contracts;
using NLog;

namespace MineSweeper.ViewModels;

public partial class AppViewModel : ObservableRecipient
{
    [ObservableProperty]
    private IGameState _game;

    [ObservableProperty]
    private ITurnProcess _turn;

    private readonly ILogger _logger;

    private readonly IConsoleOut _consoleOut;

    public AppViewModel(IGameState game, ITurnProcess turn, ILogger logger, IConsoleOut consoleOut)
    {
        _game = game;
        _turn = turn;
        _logger = logger;

        _logger.Info("Game Loaded!");
        _consoleOut = consoleOut;
    }

    [ICommand]
    private void OpenConsole(object args)
    {
        _consoleOut.CloseConsole();

        _consoleOut.LoadConsole();
    }

    [ICommand]
    private void CloseConsole(object args)
    {
        _consoleOut.CloseConsole();
    }
}
