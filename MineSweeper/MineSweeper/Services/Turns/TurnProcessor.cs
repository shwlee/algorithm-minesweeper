using MineSweeper.Contracts;
using System;

namespace MineSweeper.Services.Turns;

public class TurnProcessor : ITurnProcess
{
    private readonly IGameState _gameState;

    private readonly IPlayerLoader _playerLoader;

    public TurnProcessor(IGameState gameState, IPlayerLoader playerLoader)
    {
        _gameState = gameState;
        _playerLoader = playerLoader;
    }

    public void LoadPlayers()
    {
        _playerLoader.LoadPlayers();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }
}
