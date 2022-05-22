using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Contracts;
using MineSweeper.Exceptions;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    private bool _usePlayers;

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

    /// <summary>
    /// Player 에게 전달할 보드 현황. 매 플레이어 호출 직후 업데이트한다.
    /// </summary>
    private int[]? _board;

    private Box[,]? _composition;

    private List<Box> _boxList = new List<Box>();

    private List<MinePosition> _startingArea = new List<MinePosition>(16);

    [ObservableProperty]
    private ObservableCollection<Box>? _boxes;

    public GameViewModel()
    {
        this.IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<GameViewModel, OpenBoxMessage>(this, (r, m) => r.OpenBox(m.Target));
        Messenger.Register<GameViewModel, MarkBoxMessage>(this, (r, m) => r.MarkBox(m.Target));
    }

    public int[]? GetBoard()
    {
        return _board?.ToArray();
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

            if (box.IsMine)
            {
                return -2;
            }

            if (box.IsMarked)
            {
                return -3;
            }

            return -1;
        });

        _board = board.ToArray();
    }


    private void OpenBox(Box box)
    {
        try
        {
            if (box.IsMarked) // marking 된 box 는 열지 않는다.
            {
                return;
            }

            box.IsOpened = true;

            if (box.IsMine)
            {
                foreach (var otherBox in _boxList)
                {
                    otherBox.IsOpened = true;
                }

                // game over
                return;
            }

            if (box.Number is 0)
            {
                // 인근 박스를 모두 연다.            
                OpenAroundBoxes(box.X, box.Y);
            }

            UpdateBoardData();

            Verify();
        }
        catch (GameOverException gameOver)
        {
            // TODO : logging.

            // TODO : 게임 종료 처리. (정산. 게임 종료 애니메이션 등.)
        }
    }

    private void MarkBox(Box box)
    {
        if (box.IsOpened)
        {
            return;
        }

        box.IsMarked = box.IsMarked == false;

        UpdateBoardData();
    }

    private void OpenAroundBoxes(int column, int row)
    {
        var aroundBoxes = GetAroundBoxes(column, row);
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
            if (aroundBox.Number is 0)
            {
                OpenAroundBoxes(aroundBox.X, aroundBox.Y);
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
        // 일단 player 가 4명이라고 가정. TODO : player 수에 맞게 인자로 받아서 처리.
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

    private void Verify()
    {
        // TODO : 게임 정상 상태 판단
    }
}
