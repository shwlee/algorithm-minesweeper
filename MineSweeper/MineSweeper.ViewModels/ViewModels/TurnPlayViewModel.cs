using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Commons.Extensions;
using MineSweeper.Defines.Games;
using MineSweeper.Defines.Utils;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using MineSweeper.Models.Models.Messages;
using MineSweeper.ViewModels.Exceptions;
using NLog;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

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
            if (SetProperty(ref _turnCount, value))
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
        _logger = logger;
        IsActive = true;

        _logger.Info("Turn created!");
    }

    protected override void OnActivated()
    {
        Messenger.Register<TurnPlayViewModel, GameMessage>(this, (r, m) => r.GameMessage(m));
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
                
                // TODO : player load / clear 에 대한 별도 기능 필요.
                LoadPlayers();

                _logger.Info($"Player loaded. {string.Join(",", Players!.Select(player => $"{player.Index}-{player.Name}"))}");
                break;
            case GameStateMessage.Start:
                break;
            case GameStateMessage.GameOver:
                await StopAutoPlay();

                // 사실상 없다.
                GameOver();
                
                // TODO : 후처리.
                break;
        }
    }

    private void GameOver([Optional] int? outPlayer)
    {
        if (Players is null)
        {
            return;
        }

        foreach (var player in Players!)
        {
            if (player is null)
            {
                continue;
            }

            if (player.IsOutPlayer)
            {
                player.Score = 0; // 이미 out 이면 한 번 더 0 처리.
                continue;
            }

            if (outPlayer is not null)
            {
                if (player.Index == outPlayer)
                {
                    player.Score = 0; // 탈락 / 지뢰 밟으면 0점 처리.
                    player.IsOutPlayer = true;
                    continue;
                }
            }

            var score = _gameState.GetResultScore(player.Index);
            player.Score = score;
        }

        var winnerMessage = new WinnerPopupMessage(Players.ToList());
        Messenger.Send(winnerMessage);

        _logger.Info($"Game Over. turn:{TurnCount}. players: {string.Join(",", Players!.Select(player => $"[{player.Index}] {player.Name}({player.Score})"))}");
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
    private async void TurnOne()
    {
        try
        {
            var board = GetCurrentBoard();

            await ExecuteTurn(board, false);

            // 수동턴에 의한 lastTurnPlayer 와 TurnCount 보정.
            if (_lastTurnPlayer >= Players!.Count - 1)
            {
                _lastTurnPlayer = 0;
                TurnCount++;
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
            // TODO : 후처리?
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
            _logger.Error(ex);
            CanControlPlay = true;
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
                    _logger.Info($"Stop auto play");
                    break;
                case AutoPlay.X1:
                    StartAutoPlay();
                    _logger.Info($"Start auto play. speed: x1");
                    break;
                case AutoPlay.X2:
                case AutoPlay.X3:
                    break;
            }
        }
        finally
        {
            AutoSpeed = currentSpeed;
            _logger.Info($"Set auto play speed: {AutoSpeed}");
        }
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
                    break;
                }
            }
        }, cancelToken);
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

    private void CancelAutoPlayer()
    {
        try
        {
            _autoPlayCancelTokenSource?.Cancel();
            _autoPlayCancelTokenSource?.Dispose();
            _autoPlayCancelTokenSource = new CancellationTokenSource();

            _logger.Info($"Cancel auto play.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex);
        }
    }
}
