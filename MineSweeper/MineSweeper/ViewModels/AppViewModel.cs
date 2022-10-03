using CommunityToolkit.Mvvm.ComponentModel;
using MineSweeper.Contracts;

namespace MineSweeper.ViewModels;

public partial class AppViewModel : ObservableRecipient
{
    [ObservableProperty]
    private IGameState _game;

    [ObservableProperty]
    private ITurnProcess _turn;

    public AppViewModel(IGameState game, ITurnProcess turn)
    {
        _game = game;
        _turn = turn;
    }


}
