// sample

var _myNumber;
var _column;
var _row;
var _totalMineCount;

// 초기화 함수.
// int: myNumber = 할당받은 플레이어 번호. (플레이 순서)
// int: column = 현재 생성된 보드의 열.
// int: row = 현재 생성된 보드의 행.
function Initialize(myNumber, column, row, totalMineCount) {
    _myNumber = myNumber;
    _column = column;
    _row = row;
    _totalMineCount = totalMineCount;
}

// 플레이어 이름을 반환.
function GetName() {
    return "NXP Greg javascript!";
}

// 각 턴의 행동을 결정.
// int[]: board = 현재 보드의 정보. 자세한 내용은 아래 주석 참조.
// int: turneCount = 현재 턴.
function Turn(board, turnCount) {
    let action = Math.random() * 2;
    let position = Math.random() * (board.length - 1);
    return {
        action,
        position
    };
}

/*
   int[] borad 내용
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




    Turn 함수 리턴값
    return {
        action,
        position
    }
    
        이번 턴에서 수행할 동작을 의미한다.

        int: action = 동작 내용.
            지정한 위치의 box 를 연다.
            0,

            지정한 위치의 box에 marking 한다.
            1,

            턴을 끝내고 게임이 종료될 때까지 더이상 플레이 하지 않는다.
            2

        int: position = 위 Action 을 수행할 대상 box 의 위치.
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

    return {
        action = 0,
        position = 9
    }; // (4,2) 위치의 box 를 연다.


*/