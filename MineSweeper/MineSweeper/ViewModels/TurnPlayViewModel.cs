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
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MineSweeper.ViewModels;

public partial class TurnPlayViewModel : ObservableRecipient, ITurnProcess
{
    private IGameState _gameState;

    private IPlayerLoader _playerLoader;

    private int _lastTurnPlayer;

    [ObservableProperty]
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
        var players = loadedPlayers.OrderBy(player => new Random().Next(columns * rows))
                        .Select((player, i) => new TurnPlayer(player, i))
                        .ToList(); // random 배치.

        foreach (var player in players)
        {
            player.Turn.Initialize(player.Index, columns, rows);
        }

        Players = new ObservableCollection<TurnPlayer>(players);
    }

    [ICommand]
    private void TurnOne()
    {
        try
        {
            var board = GetCurrentBoard();

            if (_lastTurnPlayer >= Players!.Count - 1)
            {
                _lastTurnPlayer = 0;
                TurnCount++;
            }

            var player = Players[_lastTurnPlayer];
            if (player.IsClosePlayer)
            {
                return;
            }

            var action = player.Turn.Turn(board, TurnCount);
            if (action.Action is Player.PlayerAction.Close)
            {
                player.IsClosePlayer = true;
                return;
            }

            _gameState.Set(action, player.Index);
        }
        catch (Exception ex)
        {
            // TODO : logger
            // TODO : 후처리.
        }
        finally
        {
            _lastTurnPlayer++;
        }
    }



    [ICommand]
    private void TurnAll()
    {
        _ = ExecuteTurnAll();
    }

    private async Task ExecuteTurnAll([Optional] AutoPlay? playSpeed)
    {
        try
        {
            if (Players is null)
            {
                throw new GameNotInitializedExceptionException();
            }

            foreach (var player in Players)
            {
                // 수동 턴 이후 호출되었을 때, 현재 player 와 lastTurnPlayer 를 맞춘다.
                var currentIndex = Players.IndexOf(player);
                if (currentIndex < _lastTurnPlayer)
                {
                    continue;
                }

                var board = GetCurrentBoard();
                try
                {
                    if (player.IsClosePlayer)
                    {
                        continue;
                    }

                    var action = player.Turn.Turn(board, TurnCount);
                    if (action.Action is Player.PlayerAction.Close)
                    {
                        player.IsClosePlayer = true;
                        continue;
                    }

                    _gameState.Set(action, player.Index);

                    if (_autoPlayCancelTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    if (_gameState.IsGameOver())
                    {
                        return;
                    }

                    var speed = playSpeed is null or AutoPlay.Stop ? 1000 : 1000 / (int)playSpeed.Value;
                    await Task.Delay(speed, _autoPlayCancelTokenSource.Token);
                }
                finally
                {
                    _lastTurnPlayer = _lastTurnPlayer >= Players.Count - 1 ? 0 : _lastTurnPlayer + 1;
                }
            }

            TurnCount++;
        }
        catch (Exception ex)
        {
            // TODO : logger
            // TODO : 후처리.
        }
    }

    private bool _isAutoPlaying;

    private CancellationTokenSource _autoPlayCancelTokenSource = new CancellationTokenSource();

    [ICommand]
    private void AutoTurn()
    {
        if (_gameState.IsInitialized is false)
        {
            return;
        }

        if (_isAutoPlaying)
        {
            var index = (int)AutoSpeed;
            var up = (++index) % 4;
            var speedUp = (AutoPlay)up;

            AutoSpeed = speedUp;
            if (AutoSpeed is AutoPlay.Stop)
            {
                _autoPlayCancelTokenSource.Cancel();
            }
        }
        else
        {
            var cancelToken = _autoPlayCancelTokenSource.Token;
            var turns = Task.Run(async () =>
            {
                while (true)
                {
                    if (_gameState.IsGameOver())
                    {
                        break;
                    }

                    await ExecuteTurnAll(AutoSpeed);

                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    await Task.Delay(1000, cancelToken); // 모든 플레이어가 턴을 돌고 1초를 더 쉰다.
                }

                _isAutoPlaying = false;
            }, cancelToken);

            _isAutoPlaying = true;
        }
    }

    private int[] GetCurrentBoard()
    {
        if (_gameState.IsInitialized is false)
        {
            throw new GameNotInitializedExceptionException();
        }

        if (Players is null)
        {
            throw new GameNotInitializedExceptionException();
        }

        var board = _gameState.GetBoard();
        if (board is null)
        {
            throw new GameNotInitializedExceptionException();
        }

        return board;
    }

    public void Start()
    {
    }
}
