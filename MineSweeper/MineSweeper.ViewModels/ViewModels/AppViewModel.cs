using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Defines.Games;
using MineSweeper.Defines.Utils;
using MineSweeper.Models.Models.Messages;
using NLog;

namespace MineSweeper.ViewModels;

public partial class AppViewModel : ObservableRecipient
{
    [ObservableProperty]
    private IGameState _game;

    [ObservableProperty]
    private ITurnProcess _turn;

    [ObservableProperty]
    private INotificationPopup _popup;

    private readonly ILogger _logger;

    private readonly IConsoleOut _consoleOut;

    public AppViewModel(IGameState game, ITurnProcess turn, INotificationPopup popup, ILogger logger, IConsoleOut consoleOut)
    {
        _game = game;
        _turn = turn;
        _popup = popup;

        _logger = logger;
        _logger.Info("Game Loaded!");

        _consoleOut = consoleOut;

        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<AppViewModel, WinnerPopupMessage>(this, (r, m) => r.PopupGameOver(m));
        Messenger.Register<AppViewModel, NotificationCloseMessage>(this, (r, m) => r.CloseNotification());
    }

    private void CloseNotification()
    {
        _popup.IsPopup = false;
        _popup.Content = null;

        _logger.Info("Close notification panel.");
    }

    private void PopupGameOver(WinnerPopupMessage message)
    {
        var winner = new GameOverViewModel(message.Players, _logger);
        _popup.Content = winner;
        _popup.IsPopup = true;

        _logger.Info("Notification panel popped up!");
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
