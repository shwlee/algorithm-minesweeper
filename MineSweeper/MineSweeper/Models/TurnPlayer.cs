using Microsoft.Toolkit.Mvvm.ComponentModel;
using MineSweeper.Player;

namespace MineSweeper.Models;

public partial class TurnPlayer : ObservableObject
{
    public IPlayer Turn { get; }

    public int Index { get; init; }

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private int _score;

    public bool IsClosePlayer { get; set; }

    public TurnPlayer(IPlayer player, int index)
    {
        Turn = player;
        Index = index;
        _name = player.GetName();
    }
}
