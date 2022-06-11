﻿using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Defines.Games;
using MineSweeper.Models;
using MineSweeper.Models.Models.Messages;
using NLog;
using System.Collections.ObjectModel;

namespace MineSweeper.ViewModels;

public partial class WinnerViewModel : ObservableRecipient, IPopupContent
{
    private readonly ILogger _logger;

    [ObservableProperty]
    public ObservableCollection<TurnPlayer> _players;

    [ObservableProperty]
    public TurnPlayer? _winner; // 동점자가 있을 경우 뒤쪽 순번이 승리.    

    public WinnerViewModel(IEnumerable<TurnPlayer> players, ILogger logger)
    {
        _players = new ObservableCollection<TurnPlayer>(players);
        _winner = Players?.OrderByDescending(player => player.Index).MaxBy(player => player.Score);
        _logger = logger;

        _logger.Info($"Game set! winner:{_winner?.Name}");
        _logger.Info($"Player board: {string.Join(",", _players.Select(player => $"[{player.Name}:{player.Score}]"))}");
    }

    [ICommand]
    private void Close(object args)
    {
        Messenger.Send(new NotificationCloseMessage());
    }
}
