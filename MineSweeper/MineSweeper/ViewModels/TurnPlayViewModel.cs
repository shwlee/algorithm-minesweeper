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
        _players = new ObservableCollection<TurnPlayer>(players);
    }

    void ITurnProcess.LoadPlayers()
    {
        throw new System.NotImplementedException();
    }

    public void Start()
    {
        throw new System.NotImplementedException();
    }
}
