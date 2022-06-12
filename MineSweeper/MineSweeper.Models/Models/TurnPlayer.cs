using Microsoft.Toolkit.Mvvm.ComponentModel;
using MineSweeper.Player;

namespace MineSweeper.Models;

public partial class TurnPlayer : ObservableObject
{
    public IPlayer Turn { get; }

    [ObservableProperty]
    public int _index;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private int _score;

    [ObservableProperty]
    private bool _isOutPlayer;

    [ObservableProperty]
    public bool _isClosePlayer;

    [ObservableProperty]
    public bool _isWinner;

    public TurnPlayer(IPlayer player, int index)
    {
        Turn = player;
        Index = index;
        _name = player.GetName();
    }
}
