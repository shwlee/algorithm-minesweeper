using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MineSweeper.ViewModels;

public partial class AppViewModel : ObservableObject
{
    // 4보다 커야한다.
    [ObservableProperty]
    private int _columns = 10;

    // 4보다 커야한다.
    [ObservableProperty]
    private int _rows = 10;

    [ObservableProperty]
    private int _mineCount;

    [ObservableProperty]
    private int _uniformColumns;

    [ObservableProperty]
    private int _uniformRows;

    private Box[,]? _composition;

    private List<MinePosition> _startingArea = new List<MinePosition>(16);

    public ObservableCollection<Box> Boxes { get; } = new ObservableCollection<Box>();

    [ICommand]
    private void ApplyLayout()
    {        
        Boxes.Clear();
        _startingArea.Clear();

        _composition = new Box[_columns, _rows];

        for (var i = 0; i < _columns; i++)
        {
            for (var j = 0; j < _rows; j++)
            {
                var newBlock = new Box(i, j);
                Boxes.Add(newBlock);

                _composition[i, j] = newBlock;
            }
        }

        _uniformColumns = _columns;
        _uniformRows = _rows;

        GenerateStartArea();

        SetMinePosition();
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
                //var x = new Random(DateTime.UtcNow.Millisecond + i).Next(0, _columns);
                //var y = new Random(DateTime.UtcNow.Millisecond + i).Next(0, _rows);
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
            foreach(var aroundBox in aroundBoxes)
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
        boxes[0] = left < 0 ? null : ( top < 0 ? null  : _composition[left, top]); // left top
        boxes[1] = top < 0 ? null  : _composition[column, top]; // middle top
        boxes[2] = right > _columns - 1 ? null : (top < 0 ? null  : _composition[right, top]); // right top

        boxes[3] = left < 0 ? null : _composition[left, row]; // left middle
        boxes[4] = right > _columns - 1 ? null : _composition[right, row];// right middle

        boxes[5] = left < 0 ? null : ( bottom > _rows - 1 ? null  : _composition[left, bottom]); ;// left bottom
        boxes[6] = bottom > _rows - 1 ? null  : _composition[column, bottom];// middle bottom
        boxes[7] = right > _columns - 1 ? null : (bottom > _rows - 1 ? null  : _composition[right, bottom]);// right bottom

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
