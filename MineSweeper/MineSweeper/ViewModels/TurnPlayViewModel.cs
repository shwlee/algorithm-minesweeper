using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using MineSweeper.Contracts;
using MineSweeper.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MineSweeper.ViewModels;

public partial class TurnPlayViewModel : ObservableObject, ITurnProcess
{
    private IGameState _gameState;

    private IPlayerLoader _playerLoader;

    [ObservableProperty]
    private AutoPlay _autoSpeed;

    [ObservableProperty]
    private ObservableCollection<TurnPlayer>? _players;

    public TurnPlayViewModel(IGameState gameState, IPlayerLoader playerLoader)
    {
        _gameState = gameState;
        _playerLoader = playerLoader;
    }

    [ICommand]
    private void LoadPlayers()
    {
        var loadedPlayers = _playerLoader.LoadPlayers();
        var players = loadedPlayers.Select((player, i) => new TurnPlayer(player, i));
        Players = new ObservableCollection<TurnPlayer>(players);
    }

    [ICommand]
    private void Turn()
    {

    }

    [ICommand]
    private void AutoTurn()
    {

    }

    public void Start()
    {
    }
}
