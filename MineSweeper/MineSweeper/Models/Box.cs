using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MineSweeper.Models;

public partial class Box : ObservableObject
{
    /// <summary>
    /// 지뢰 여부.
    /// </summary>
    [ObservableProperty]
    private bool _isMine;

    /// <summary>
    /// 지뢰로 표시 여부. (열지 않은 상태)
    /// </summary>
    [ObservableProperty]
    private bool _isMarked;

    /// <summary>
    /// 지뢰가 아닐 경우 주변 8칸에 존재하는 지뢰의 개수
    /// </summary>
    [ObservableProperty]
    private int _number;

    /// <summary>
    /// box 를 열었는지 여부.
    /// </summary>
    [ObservableProperty]
    private bool _isOpened;

    /// <summary>
    /// box 를 열거나 mark 한 player.
    /// </summary>
    [ObservableProperty]
    private int _owner;

    public Box(int column, int row)
    {
        X = column;
        Y = row;
    }

    /// <summary>
    /// 전체 보드에서 열 인덱스.
    /// </summary>
    public int X { get; }

    /// <summary>
    /// 전체 보드에서 행 인덱스.
    /// </summary>
    public int Y { get; }
}
