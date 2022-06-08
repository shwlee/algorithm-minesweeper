using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Contracts;
using MineSweeper.Exceptions;
using MineSweeper.Extensions;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MineSweeper.ViewModels;

public partial class TurnPlayViewModel : ObservableRecipient, ITurnProcess
{
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

    private async Task ExecuteTurn(int[] board, [Optional] bool useException)
    {
        try
        {
            var player = Players![_lastTurnPlayer];
            if (player.IsClosePlayer)
            {
                throw new TurnContinueException();
            }

            var action = await Task.Run(() => player.Turn.Turn(board, TurnCount)).Timeout(TimeSpan.FromMilliseconds(3000)); // TODO : to config            
            if (action.Action is Player.PlayerAction.Close)
            {
                player.IsClosePlayer = true;
                throw new TurnContinueException();
            }

            _gameState.Set(action, player.Index);

            player.Score = _gameState.GetScore(player.Index);
        }
        catch (TimeoutException timeout)
        {
            // TODO : 탈락 처리.
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
                    await ExecuteTurn(board);

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
            throw;
            // TODO : logger
            // TODO : 후처리.
        }
    }
}
