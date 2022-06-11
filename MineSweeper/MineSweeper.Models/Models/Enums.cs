namespace MineSweeper.Models;

public enum Platform
{
    CS = 0,

    CPP,

    Javascript,

    Python
}

public enum AutoPlay
{
    Stop = 0,
    
    X1 = 1,
    
    X2 = 2,
    
    X3 = 3,
}

public enum GameStateMessage
{
    Set,

    Start,

    GameOver
}
