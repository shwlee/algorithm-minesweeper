namespace MineSweeper.Player;

public enum PlayerAction
{
    /// <summary>
    /// 지정한 위치의 box 를 연다.
    /// </summary>
    Open = 0,

    /// <summary>
    /// 지정한 위치의 box에 marking 한다.
    /// </summary>
    Mark = 1,

    /// <summary>
    /// 턴을 끝내고 게임이 종료될 때까지 더이상 플레이 하지 않는다.
    /// </summary>
    Close = 2
}
