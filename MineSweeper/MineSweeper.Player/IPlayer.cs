namespace MineSweeper.Player;

public interface IPlayer
{
    void Initialize(int myNumber, int column, int row);

    /// <summary>
    /// 현재 턴의 동작을 결정한다.
    /// </summary>
    /// <param name="board">배치 현황. 전체 배치 정보가 1차원 배열로 할당되어 전달된다. 자세한 정보는 아래 참조.</param>
    /// <param name="turnCount">현재 턴. 턴은 1부터 시작한다.</param>
    /// <param name="myScore">현재 턴 이전까지 자신의 점수.</param>
    /// <returns>현재 턴 동작.</returns>
    PlayContext Turn(int[] board, int turnCount, int myScore);

    /*
    int[] borad 내용
        1. 현재 보드의 전체 box 를 1차원 배열로 전달한다.
            int 값으로 구성된다.
            0 부터 시작한다.
        2. 배열 요소
            0 >= item : 해당 box 주변에 배치된 mime 의 개수.
            0 < item : box state
            -1 : unopened
            -2 : mine
            -3 : mark
    */
}
