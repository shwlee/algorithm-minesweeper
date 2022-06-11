using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Defines.Games;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using MineSweeper.Player;
using MineSweeper.ViewModels.Exceptions;
using NLog;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace MineSweeper.ViewModels;

public partial class GameViewModel : ObservableRecipient, IGameState
{
    // TODO : validation 필요.

    // 최소 4보다 커야한다.
    [ObservableProperty]
    private int _columns = 10;

    // 최소 4보다 커야한다.
    [ObservableProperty]
    private int _rows = 10;

    [ObservableProperty]
    private int _mineCount;

    [ObservableProperty]
    private int _uniformColumns;

    [ObservableProperty]
    private int _uniformRows;

    [ObservableProperty]
    private bool _usePlayers = true;

    private bool _isShowAll;
    public bool IsShowAll
    {
        get => _isShowAll;
        set
        {
            SetProperty(ref _isShowAll, value);
            foreach (var box in _boxList)
            {
                box.IsOpened = value;
            }
        }
    }

    private bool _isInitialized;
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Player 에게 전달할 보드 현황. 매 플레이어 호출 직후 업데이트한다.
    /// </summary>
    private int[]? _board;

    private Box[,]? _composition;

    private List<Box> _boxList = new List<Box>();

    private List<MinePosition> _startingArea = new List<MinePosition>(16);

    [ObservableProperty]
    private ObservableCollection<Box>? _boxes;

    private readonly ILogger _logger;

    public GameViewModel(ILogger logger)
    {
        this.IsActive = true;
        _logger = logger;

        _logger.Info("Game created!");
    }

    protected override void OnActivated()
    {
        // 수동으로 조작했을 때 처리하기 위한 옵션.
        Messenger.Register<GameViewModel, OpenBoxMessage>(this, (r, m) => r.OpenBox(m.Target));
        Messenger.Register<GameViewModel, MarkBoxMessage>(this, (r, m) => r.MarkBox(m.Target));
    }

    public int[]? GetBoard()
    {
        return _board?.ToArray();
    }

    public (int column, int row) GetColumRows()
    {
        if (_isInitialized is false)
        {
            throw new GameNotInitializedExceptionException();
        }

        return (_columns, _rows);
    }

    public void Set(PlayContext context, int playerIndex)
    {
        var position = context.Position;
        var box = _boxList[position];
        if (box is null)
        {
            var reason = context.Action switch
            {
                PlayerAction.Open => ViolationReason.TryOpenOutOfRange,
                PlayerAction.Mark => ViolationReason.TryMarkOutOfRange,
                _ => throw new TurnContinueException(playerIndex)
            };

            throw new MineSweepViolationException(playerIndex, reason);
        }

        var action = context.Action;
        switch (action)
        {
            case PlayerAction.Open:
                OpenBox(box, playerIndex);
                break;
            case PlayerAction.Mark:
                MarkBox(box, playerIndex);
                break;
            case PlayerAction.Close:
                break;
        }
    }

    public bool IsGameOver()
    {
        // mine 이 하나도 없으면 mine 설정이 잘못 되었다.
        if (_boxList.Any(box => box.IsMine) is false)
        {
            throw new GameNotInitializedExceptionException();
        }

        // mine 을 열었는지만 확인.
        if (_boxList.Any(box => box.IsMine && box.IsOpened))
        {
            return true;
        }

        // 열지 않은 박스가 남아 있으면 계속 진행.
        if (_boxList.Any(box => box.IsOpened is false))
        {
            return false;
        }

        // 남은 모든 mine box 가 mark 일 때
        if (_boxList.Where(box => box.IsMine).All(box => box.IsMarked))
        {
            return true;
        }

        return false;
    }

    public int GetNumberOfTotalMines()
    {
        return _mineCount;
    }

    public int GetScore(int playerIndex)
    {
        var score = _boxList.Where(box => box.Owner == playerIndex && box.IsMarked is false).Sum(box => box.Number);
        return score;
    }

    public int GetResultScore(int playerIndex)
    {
        var inGameScore = GetScore(playerIndex);
        var resultScore = _boxList.Count(box => box.Owner == playerIndex && box.IsMarked) * 5; // mark 1개당 5점 추가.
        return resultScore + inGameScore;
    }

    [ICommand]
    private void ApplyLayout()
    {
        _boxList.Clear();
        _startingArea.Clear();
        Boxes?.Clear();

        _composition = new Box[_columns, _rows];

        for (var j = 0; j < _rows; j++)
        {
            for (var i = 0; i < _columns; i++)
            {
                var newBlock = new Box(i, j);
                _boxList.Add(newBlock);

                _composition[i, j] = newBlock;
            }
        }

        Boxes = new ObservableCollection<Box>(_boxList);

        UniformColumns = _columns;
        UniformRows = _rows;

        if (_usePlayers)
        {
            GenerateStartArea();
        }

        SetMinePosition();

        InitBoardData();

        _isInitialized = true;

        Messenger.Send(new GameMessage(GameStateMessage.Set));

        _logger.Info($"Game init: columns:{_columns}, rows:{_rows}, mines:{_mineCount}");
        _logger.Info($"board intialized: [{string.Join(",", _boxList.Select(box => box.IsMine ? -3 : box.Number))}]");
    }

    private void InitBoardData()
    {
        // 전부 닫힌 box 로 초기화 한다.
        var total = _columns * _rows;
        _board = new int[total];
        for (var i = 0; i < total; i++)
        {
            _board[i] = -1;
        }
    }

    private void UpdateBoardData()
    {
        var board = _boxList.Select(box =>
        {
            if (box.IsOpened)
            {
                return box.Number;
            }

            if (box.IsMarked)
            {
                return -2;
            }

            return -1;
        });

        _board = board.ToArray();
        _logger.Info($"board state: [{string.Join(",", _board)}]");
    }


    private void OpenBox(Box box, [Optional] int? player)
    {
        try
        {
            if (box.IsMine)
            {
                foreach (var otherBox in _boxList)
                {
                    otherBox.IsOpened = true;
                }

                _logger.Info($"Hit a mine. Game Over! player:{player}, box:{box.X + (box.Y * _columns)} ({box.X}, {box.Y})");

                // game over
                throw new GameOverException(player);
            }

            if (box.IsMarked) // marking 된 box 는 열지 않는다.
            {
                _logger.Info($"Tried mark the box already marked. player:{player}, box:{box.X + (box.Y * _columns)} ({box.X}, {box.Y})");
                throw new MineSweepViolationException(player, ViolationReason.TryOpenAlreadyMarked);
            }

            if (box.IsOpened)
            {
                _logger.Info($"Tried open the box already opened. player:{player}, box:{box.X + (box.Y * _columns)} ({box.X}, {box.Y})");
                throw new MineSweepViolationException(player, ViolationReason.TryOpenAlreadyOpened);
            }

            UnselectOpener(player);

            box.IsOpened = true;

            _logger.Info($"Box opened. player:{player}, box:{box.X + (box.Y * _columns)} ({box.X}, {box.Y})");
            box.SelectedOpener = player ?? -1;
            if (player is not null)
            {
                box.Owner = player.Value;
                _logger.Info($"Player got the score. player:{player}, box:{box.X + (box.Y * _columns)} ({box.X}, {box.Y}), point:{box.Number}");
            }

            if (box.Number is 0)
            {
                // 0을 찍으면 인근 박스를 모두 연다.            
                OpenAroundBoxes(box.X, box.Y, player);
            }

            UpdateBoardData();

            Verify(player);
        }
        catch (Exception ex)
        {
            _isInitialized = false;

            _logger.Error(ex);

            throw;
        }
    }

    private void UnselectOpener(int? player)
    {
        var opener = _boxList.FirstOrDefault(box => box.SelectedOpener == player);
        if (opener is null)
        {
            return;
        }

        opener.SelectedOpener = -1;
    }

    private void MarkBox(Box box, [Optional] int? player)
    {
        try
        {
            if (box.IsOpened)
            {
                return;
            }

            // mark, demark 모두 가능
            box.IsMarked = box.IsMarked == false;

            _logger.Info($"Box marked. player:{player}, box:{box.X + (box.Y * _columns)} ({box.X}, {box.Y}), marked state:{box.IsMarked}");

            box.SelectedMarker = box.IsMarked ? player ?? -1 : -1;
            if (player is not null)
            {
                box.Owner = player.Value;
            }

            UpdateBoardData();

            Verify(player);
        }
        catch (Exception ex)
        {
            _isInitialized = false;

            _logger.Error(ex);

            throw;
        }
    }

    private void OpenAroundBoxes(int column, int row, int? player)
    {
        var aroundBoxes = GetAroundBoxes(column, row);
        _logger.Info($"Player {player} select [{column + (row * _columns)}] around boxes. " +
            $"boxes:{string.Join(",", aroundBoxes.Select(box => box?.X + (box?.Y * _columns)))}");
        foreach (var aroundBox in aroundBoxes)
        {
            if (aroundBox is null)
            {
                continue;
            }

            if (aroundBox.IsOpened) // 이미 열려 있으면 스킵.
            {
                continue;
            }

            aroundBox.IsOpened = true;

            _logger.Info($"Box opened. player:{player}, box:{column + (row * _columns)} ({column}, {row})");

            if (player is not null)
            {
                aroundBox.Owner = player.Value;
            }

            if (aroundBox.Number is 0)
            {
                OpenAroundBoxes(aroundBox.X, aroundBox.Y, player);
            }
        }
    }

    private void SetMinePosition()
    {
        if (_composition is null)
        {
            throw new InvalidOperationException("The mine box composition is not set yet.");
        }

        var minePositions = new List<MinePosition>(_mineCount);
        for (var i = 0; i < _mineCount; i++)
        {
            while (i < _mineCount)
            {
                var x = new Random().Next(0, _columns);
                var y = new Random().Next(0, _rows);
                var position = new MinePosition(x, y);

                if (minePositions.Any(mine => mine == position))
                {
                    continue;
                }

                if (_startingArea.Any(mine => mine == position))
                {
                    continue;
                }

                // 겹치지 않는 위치 생성.
                minePositions.Add(position);

                break;
            }
        }

        foreach (var minePosition in minePositions)
        {
            var column = minePosition.X;
            var row = minePosition.Y;
            var box = _composition[column, row];
            box.IsMine = true;

            var aroundBoxes = GetAroundBoxes(column, row);
            foreach (var aroundBox in aroundBoxes)
            {
                if (aroundBox is null)
                {
                    continue;
                }

                aroundBox.Number++;
            }
        }
    }

    private Box?[] GetAroundBoxes(int column, int row)
    {
        if (_composition is null)
        {
            throw new InvalidOperationException("The mine box composition is not set yet.");
        }

        var boxes = new Box?[8];
        var left = column - 1;
        var top = row - 1;
        var right = column + 1;
        var bottom = row + 1;

        // 정해진 column / row 를 둘러싼 8개 box 를 획득한다.
        boxes[0] = left < 0 ? null : (top < 0 ? null : _composition[left, top]); // left top
        boxes[1] = top < 0 ? null : _composition[column, top]; // middle top
        boxes[2] = right > _columns - 1 ? null : (top < 0 ? null : _composition[right, top]); // right top

        boxes[3] = left < 0 ? null : _composition[left, row]; // left middle
        boxes[4] = right > _columns - 1 ? null : _composition[right, row];// right middle

        boxes[5] = left < 0 ? null : (bottom > _rows - 1 ? null : _composition[left, bottom]); ;// left bottom
        boxes[6] = bottom > _rows - 1 ? null : _composition[column, bottom];// middle bottom
        boxes[7] = right > _columns - 1 ? null : (bottom > _rows - 1 ? null : _composition[right, bottom]);// right bottom

        return boxes;
    }

    private void GenerateStartArea()
    {
        // 금지구역 설정.
        // player 수와 관계없이 4 귀퉁이 영역은 시작 구역이 된다.

        var aroundLeftTop = new MinePosition[]
        {
            new MinePosition(0, 0),
            new MinePosition(1, 0),
            new MinePosition(1, 1),
            new MinePosition(0, 1),
        };
        _startingArea.AddRange(aroundLeftTop);

        var right = _columns - 1;
        var aroundRightTop = new MinePosition[]
        {
            new MinePosition(right - 1, 0),
            new MinePosition(right, 0),
            new MinePosition(right - 1, 1),
            new MinePosition(right, 1)
        };
        _startingArea.AddRange(aroundRightTop);

        var bottom = _rows - 1;
        var aroundLeftBottom = new MinePosition[]
        {
            new MinePosition(0, bottom - 1),
            new MinePosition(1, bottom -1),
            new MinePosition(1, bottom),
            new MinePosition(0, bottom),
        };
        _startingArea.AddRange(aroundLeftBottom);

        var aroundRightBottom = new MinePosition[]
        {
            new MinePosition(right- 1, bottom - 1),
            new MinePosition(right, bottom - 1),
            new MinePosition(right - 1, bottom),
            new MinePosition(right, bottom),
        };
        _startingArea.AddRange(aroundRightBottom);
    }

    private void Verify(int? player)
    {
        if (IsGameOver())
        {
            // TODO : log

            _logger.Info($"Game is over in this turn. player:{player}");

            throw new GameOverException(player);
        }
    }
}
