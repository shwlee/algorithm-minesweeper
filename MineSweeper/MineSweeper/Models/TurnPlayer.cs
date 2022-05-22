using Microsoft.Toolkit.Mvvm.ComponentModel;
using MineSweeper.Player;

namespace MineSweeper.Models;

public partial class TurnPlayer : ObservableObject
{
    public IPlayer Turn { get; }

    private int _index;

    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private int _score;

    public TurnPlayer(IPlayer player, int index)
    {
        Turn = player;
        _index = index;
        _name = player.GetName();
    }
}
