using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using MineSweeper.Models;
using MineSweeper.Models.Messages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MineSweeper.ViewModels;

public partial class AppViewModel : ObservableRecipient
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
            foreach (var box in Boxes)
            {
                box.IsOpened = value;
            }
        }
    }

    private Box[,]? _composition;

    private List<MinePosition> _startingArea = new List<MinePosition>(16);

    public ObservableCollection<Box> Boxes { get; } = new ObservableCollection<Box>();

    public AppViewModel()
    {
        this.IsActive = true;
    }

    protected override void OnActivated()
    {
        Messenger.Register<AppViewModel, OpenBoxMessage>(this, (r, m) => r.OpenBox(m.Opened));
    }

    [ICommand]
    private void ApplyLayout()
    {
        Boxes.Clear();
        _startingArea.Clear();

        _composition = new Box[_columns, _rows];

        for (var j = 0; j < _rows; j++)
        {
            for (var i = 0; i < _columns; i++)
            {
                var newBlock = new Box(i, j);
                Boxes.Add(newBlock);

                _composition[i, j] = newBlock;
            }
        }

        UniformColumns = _columns;
        UniformRows = _rows;

        if (_usePlayers)
        {
            GenerateStartArea();
        }

        SetMinePosition();
    }

    private void OpenBox(Box box)
    {
        box.IsOpened = true;

        if (box.IsMine)
        {
            foreach (var otherBox in Boxes)
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
}
