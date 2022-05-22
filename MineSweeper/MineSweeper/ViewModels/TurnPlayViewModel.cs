using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Contracts;
using MineSweeper.Exceptions;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MineSweeper.ViewModels;

public partial class TurnPlayViewModel : ObservableRecipient, ITurnProcess
{
    private IGameState _gameState;

    private IPlayerLoader _playerLoader;

    private int _turnCount;

    [ObservableProperty]
    private AutoPlay _autoSpeed;

    [ObservableProperty]
    private ObservableCollection<TurnPlayer>? _players;

    public TurnPlayViewModel(IGameState gameState, IPlayerLoader playerLoader)
    {
        _gameState = gameState;
        _playerLoader = playerLoader;
        IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<TurnPlayViewModel, GameMessage>(this, (r, m) => r.GameOver());
    }

    private void GameOver()
    {
        // game 정리. (player score 등)

    }

    [ICommand]
    private void LoadPlayers()
    {
        (int columns, int rows) = _gameState.GetColumRows();
        var loadedPlayers = _playerLoader.LoadPlayers();
        var players = loadedPlayers.Select((player, i) => new TurnPlayer(player, i))
                        .OrderBy(player => new Random().Next(columns * rows)); // random 배치.

        foreach (var player in players)
        {
            player.Turn.Initialize(player.Index, columns, rows);
        }

        Players = new ObservableCollection<TurnPlayer>(players);
    }

    [ICommand]
    private void Turn()
    {
        try
        {
            if (_gameState.IsInitialized is false)
            {
                throw new GameNotInitializedExceptionException();
            }

            if (Players is null)
            {
                throw new GameNotInitializedExceptionException();
            }

            foreach (var player in Players)
            {
                if (player.IsClosePlayer)
                {
                    continue;
                }

                var board = _gameState.GetBoard();
                if (board is null)
                {
                    throw new GameNotInitializedExceptionException();
                }

                var action = player.Turn.Turn(board, _turnCount);
                if (action.Action is Player.PlayerAction.Close)
                {
                    player.IsClosePlayer = true;
                    continue;
                }

                _gameState.Set(action, player.Index);
            }

            _turnCount++;
        }
        catch (Exception ex)
        {
            // TODO : logger
            // TODO : 후처리.
        }
    }

    [ICommand]
    private void AutoTurn()
    {

    }

    public void Start()
    {
    }
}
