﻿using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MineSweeper.Models;

public partial class Box : ObservableObject
{
    [ObservableProperty]
    private bool _isMine;

    [ObservableProperty]
    private bool _isMark;

    [ObservableProperty]
    private int _number;

    [ObservableProperty]
    private bool _isOpened;

    public Box(int column, int row)
    {
        Position = new MinePosition(column, row);
    }

    public MinePosition Position { get; }
}
