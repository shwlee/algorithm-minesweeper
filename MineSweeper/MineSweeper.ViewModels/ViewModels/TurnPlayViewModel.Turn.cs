using Microsoft.Toolkit.Mvvm.ComponentModel;
using MineSweeper.Commons.Extensions;
using MineSweeper.Defines.Games;
using MineSweeper.Models;
using MineSweeper.ViewModels.Exceptions;
using System.Runtime.InteropServices;

namespace MineSweeper.ViewModels;

public partial class TurnPlayViewModel : ObservableRecipient, ITurnProcess
{
    private async Task ExecuteTurn(int[] board, [Optional] bool useException)
    {
        var player = Players![_lastTurnPlayer];

        try
        {
            _logger.Info($"Turn {TurnCount} player:{player.Index} go.");

            if (player.IsOutPlayer)
            {
                throw new TurnContinueException(player.Index);
            }

            if (player.IsClosePlayer)
            {
                throw new TurnContinueException(player.Index);
            }

            var action = await Task.Run(() => player.Turn.Turn(board, TurnCount)).Timeout(TimeSpan.FromMilliseconds(3000)); // TODO : to config            
            _logger.Info($"Act. player:{player.Index}, turn:{TurnCount}, action:{action.Action}, postion:{action.Position}");

            if (action.Action is Player.PlayerAction.Close)
            {
                player.IsClosePlayer = true;

                _logger.Info($"Player set closed. turn:{TurnCount}, player:{player.Index}");

                throw new TurnContinueException(player.Index);
            }

            _gameState.Set(action, player.Index);

            player.Score = _gameState.GetScore(player.Index);

            _logger.Info($"Player {player.Index} in-game score : {player.Score}. turn:{TurnCount}");
        }
        catch (TurnContinueException continueEx)
        {
            _logger.Info($"Skip this player. player:{continueEx.Player}");

            if (useException)
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            switch (ex)
            {
                // 예외를 발생시킨 플레이어는 탈락 처리.
                case TimeoutException:
                default:
                    SetFailedPlayer(player);
                    break;
            }

            _logger.Error(ex);
        }
    }

    private void SetFailedPlayer(TurnPlayer player)
    {
        player.IsOutPlayer = true;
        player.Score = 0;

        _logger.Info($"Player set to Fail. turn:{TurnCount}, player:{player.Index}, score:{player.Score}");
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
                    await ExecuteTurn(board, true);

                    _logger.Info($"Turn player is completed. player:{player.Index}");

                    if (IsGameOver())
                    {
                        isGameOver = true;

                        GameOver();
                        break;
                    }

                    var speed = playSpeed is null or AutoPlay.Stop ? 1000 : 1000 / (int)playSpeed.Value;
                    await Task.Delay(speed);
                }
                catch (TurnContinueException turnContinue)
                {
                    if (Players.All(player => player.IsClosePlayer))
                    {
                        _logger.Info($"All players set closed. turn:{TurnCount}");
                        throw new GameOverException(null);
                    }

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
        catch (GameOverException gameOver)
        {
            GameOver(gameOver.GameOverPlayer);
        }
        catch (Exception ex)
        {
            _logger.Error(ex);

            throw;
            // TODO : 후처리.
        }
    }
}
