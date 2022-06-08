using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Contracts;
using MineSweeper.Exceptions;
using MineSweeper.Extensions;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using NLog;
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

    private IDispatcherService _dispatcherService;

    private int _lastTurnPlayer;

    private int _maxTurn;

    private int _turnCount = 1;

    private CancellationTokenSource? _autoPlayCancelTokenSource = new CancellationTokenSource();

    private Task? _autoPlay;

    private readonly ILogger _logger;

    [ObservableProperty]
    private bool _canControlPlay = true;

    [ObservableProperty]
    private bool _turnChanging;

    [ObservableProperty]
    private AutoPlay _autoSpeed;

    [ObservableProperty]
    private ObservableCollection<TurnPlayer>? _players;

    public int TurnCount
    {
        get => _turnCount;
        set
        {
            var changed = SetProperty(ref _turnCount, value);
            if (changed)
            {
                UpdateTurnChanging();
            }
        }
    }

    public TurnPlayViewModel(IGameState gameState, IPlayerLoader playerLoader, IDispatcherService dispatcherService, ILogger logger)
    {
        _gameState = gameState;
        _playerLoader = playerLoader;
        _dispatcherService = dispatcherService;
        IsActive = true;
        _logger = logger;
    }

    protected override void OnActivated()
    {
        Messenger.Register<TurnPlayViewModel, GameMessage>(this, async (r, m) => r.GameMessage(m));
    }

    private async void GameMessage(GameMessage message)
    {
        // game 정리. (player score 등)
        var state = message.State;
        switch (state)
        {
            case GameStateMessage.Set:
                _lastTurnPlayer = 0;
                AutoSpeed = AutoPlay.Stop;
                TurnCount = 1;
                // TODO : players 정리해야하나
                // player load / clear 에 대한 별도 기능 필요.
                LoadPlayers();

                break;
            case GameStateMessage.Start:
                break;
            case GameStateMessage.GameOver:
                await StopAutoPlay();

                // TODO : 후처리.
                break;
        }
    }

    private void CancelAutoPlayer()
    {
        try
        {
            _autoPlayCancelTokenSource?.Cancel();
            _autoPlayCancelTokenSource?.Dispose();
            _autoPlayCancelTokenSource = new CancellationTokenSource();
        }
        catch (Exception ex)
        {

        }
    }

    [ICommand]
    private void LoadPlayers()
    {
        (int columns, int rows) = _gameState.GetColumRows();
        _maxTurn = columns * rows;

        var loadedPlayers = _playerLoader.LoadPlayers();
        var players = loadedPlayers.OrderBy(player => new Random().Next(columns * rows))
                        .Select((player, i) => new TurnPlayer(player, i))
                        .ToList(); // random 배치.

        foreach (var player in players)
        {
            player.Turn.Initialize(player.Index, columns, rows, _gameState.GetNumberOfTotalMines());
        }

        Players = new ObservableCollection<TurnPlayer>(players);
    }

    [ICommand]
    private void TurnOne()
    {
        try
        {
            var board = GetCurrentBoard();

            ExecuteTurn(board, false);

            // 수동턴에 의한 lastTurnPlayer 와 TurnCount 보정.
            if (_lastTurnPlayer >= Players!.Count - 1)
            {
                _lastTurnPlayer = 0;
                TurnCount++;
            }
        }
        catch (Exception ex)
        {
            // TODO : logger
            // TODO : 후처리.
        }
        finally
        {
            if (Players!.Count > 1)
            {
                _lastTurnPlayer++;
            }
        }
    }

    [ICommand]
    private async void TurnAll()
    {
        try
        {
            await ExecuteTurnAll();
        }
        catch (Exception ex)
        {
            CanControlPlay = true;
        }
    }

    private async Task ExecuteTurnAll([Optional] AutoPlay? playSpeed, [Optional, DefaultParameterValue(true)] bool useControl)
    {
        try
        {
            if (Players is null)
            {
                throw new GameNotInitializedExceptionException();
            }

            if (useControl)
            {
                CanControlPlay = false;
            }

            var isGameOver = false;
            foreach (var player in Players)
            {
                // 수동 턴 이후 호출되었을 때, 현재 player 와 lastTurnPlayer 를 맞춘다.
                var currentIndex = Players.IndexOf(player);
                if (Players.Count > 1)
                {
                    if (currentIndex < _lastTurnPlayer)
                    {
                        continue;
                    }
                }

                var board = GetCurrentBoard();
                try
                {
                    ExecuteTurn(board);

                    if (IsGameOver())
                    {
                        isGameOver = true;
                        break;
                    }

                    var speed = playSpeed is null or AutoPlay.Stop ? 1000 : 1000 / (int)playSpeed.Value;
                    await Task.Delay(speed);
                }
                catch (TurnContinueException turnContinue)
                {
                    // TODO : logging
                    continue;
                }
                finally
                {
                    _lastTurnPlayer = _lastTurnPlayer >= Players.Count - 1 ? 0 : _lastTurnPlayer + 1;
                }
            }

            if (isGameOver is false)
            {
                TurnCount++;
            }

            if (useControl)
            {
                CanControlPlay = true;
            }
        }
        catch (Exception ex)
        {
            AutoSpeed = AutoPlay.Stop;
            await StopAutoPlay();

            throw;
            // TODO : logger
            // TODO : 후처리.
        }
    }

    [ICommand]
    private async void AutoTurn()
    {
        if (_gameState.IsInitialized is false)
        {
            return;
        }

        var lastSpeedIndex = (int)AutoSpeed;
        var currentSpeedIndex = (++lastSpeedIndex) % 4;
        var currentSpeed = (AutoPlay)currentSpeedIndex;

        try
        {
            switch (currentSpeed)
            {
                case AutoPlay.Stop:
                    await StopAutoPlay();
                    break;
                case AutoPlay.X1:
                    StartAutoPlay();
                    break;
                case AutoPlay.X2:
                case AutoPlay.X3:
                    break;
            }
        }
        finally
        {
            AutoSpeed = currentSpeed;
        }
    }

    private async Task StopAutoPlay()
    {
        try
        {
            CanControlPlay = false;
            AutoSpeed = AutoPlay.Stop;

            CancelAutoPlayer();

            await _autoPlay.EnsureTask();
        }
        catch (Exception ex)
        {
            // TODO : logging.
        }

        CanControlPlay = true;
    }

    private void StartAutoPlay()
    {
        if (_autoPlayCancelTokenSource is null)
        {
            throw new InvalidOperationException("The CancellationTokenSource is null");
        }

        var cancelToken = _autoPlayCancelTokenSource.Token;
        _autoPlay = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    if (IsGameOver())
                    {
                        break;
                    }

                    await ExecuteTurnAll(AutoSpeed, false);

                    if (cancelToken.IsCancellationRequested)
                    {
                        break;
                    }

                    await Task.Delay(500, cancelToken); // 모든 플레이어가 턴을 돌고 0.5초를 더 쉰다.
                }
                catch (Exception ex)
                {
                    // TODO : log

                    CancelAutoPlayer();

                    AutoSpeed = AutoPlay.Stop;
                    CanControlPlay = true;
                    return;
                }
            }
        }, cancelToken);
    }

    private void ExecuteTurn(int[] board, [Optional] bool useException)
    {
        try
        {
            var player = Players![_lastTurnPlayer];
            if (player.IsClosePlayer)
            {
                throw new TurnContinueException();
            }

            var action = player.Turn.Turn(board, TurnCount);
            if (action.Action is Player.PlayerAction.Close)
            {
                player.IsClosePlayer = true;
                throw new TurnContinueException();
            }

            _gameState.Set(action, player.Index);

            player.Score = _gameState.GetScore(player.Index);
        }
        catch (TurnContinueException continueEx)
        {
            if (useException)
            {
                throw;
            }
        }
        catch (Exception ex)
        {

            // TODO : logging;
            // TODO : 예외를 발생시킨 플레이어는 탈락 처리.
            throw;
        }
    }

    private void UpdateTurnChanging()
    {
        if (_dispatcherService.CheckAccess() is false)
        {
            _dispatcherService.Invoke(UpdateTurnChanging);

            return;
        }

        TurnChanging = true;
        TurnChanging = false;
    }

    private bool IsGameOver()
    {
        // mine 열었는 지 확인.
        if (_gameState.IsGameOver())
        {
            return true;
        }

        // 최대 턴 도달
        if (TurnCount >= _maxTurn)
        {
            return true;
        }

        // 모든 플레이어가 Close 상태.
        if (Players!.All(player => player.IsClosePlayer))
        {
            return true;
        }

        return false;
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
