namespace MineSweeper.Player;

public interface IPlayer
{
    /// <summary>
    /// Player 를 초기화 한다.
    /// </summary>
    /// <param name="myNumber">배정받은 번호.(플레이 순서)</param>
    /// <param name="column">현재 생성된 보드의 열.</param>
    /// <param name="row">현재 생성된 보드의 행.</param>
    /// <param name="totalMineCount>전체 지뢰 개수</param>
    void Initialize(int myNumber, int column, int row, int totalMineCount);

    /// <summary>
    /// Player 의 이름을 반환한다.
    /// </summary>
    /// <returns></returns>
    string GetName();

    /// <summary>
    /// 현재 턴의 동작을 결정한다.
    /// </summary>
    /// <param name="board">배치 현황. 전체 배치 정보가 1차원 배열로 할당되어 전달된다. 자세한 정보는 아래 참조.</param>
    /// <param name="turnCount">현재 턴. 턴은 1부터 시작한다.</param>    
    /// <returns>현재 턴 동작. 자세한 정보는 아래 참조.</returns>
    PlayContext Turn(int[] board, int turnCount);

    /*
    int[] borad
        1. 현재 보드의 전체 box 를 1차원 배열로 전달한다.
            int 값으로 구성된다.
            0 부터 시작한다.
        2. 배열 요소
            0 >= item : 해당 box 주변에 배치된 mime 의 개수.
            0 < item : box state
            -1 : unopened
            -2 : mark
            
        예시>
            column = 5;
            row = 5; 일 때
현재 보드
    -2  2   -2  -1  -1
    -1  2   2  -1  -1
    -1  1   2  -1  -1
    -1  2   -1  -1  -1
    -1  2   -1  4  -1    

            int[25] 짜리 배열이 전달됨.
            
            var box = int[12]; // (3,3) 위치의 box 획득

            box == 4 // 열린 box. box 주위의 4개의 mine 이 존재하는 것을 의미.
            box == -1 // 닫힌 box.            
            box == -2 // 닫힌 box. 누군가 box 에 마크했다.
    */



    /*
    PlayContext
        이번 턴에서 수행할 동작을 의미한다.

        (PlayerAction)Action : 동작 내용.
            지정한 위치의 box 를 연다.
            Open = 0,

            지정한 위치의 box에 marking 한다.
            Mark = 1,

            턴을 끝내고 게임이 종료될 때까지 더이상 플레이 하지 않는다.
            Close = 2

        (int)Position : 위 Action 을 수행할 대상 box 의 위치.
            인자로 전달받았던 int[] board 의 요소 인덱스를 지정한다.
            
    예시>
        column = 5;
        row = 5; 일 때
현재 보드
    -2  2   -2  -1  -1
    -1  2   2  -1  -1
    -1  1   2  -1  -1
    -1  2   -1  -1  -1
    -1  2   -1  4  -1    

    return new PlayerContext
    {
        Action = PlayerAction.Open,
        Position = 9
    } // (4,2) 위치의 box 를 연다.


     */
}
