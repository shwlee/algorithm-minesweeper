// sample

const playerAction = {
    Open: 0,
    Mark: 1,
    Close: 2
}

Object.freeze(playerAction);

// return 타입.
function PlayerContext(action, position) {
    this.Action = action;
    this.Position = position;
}

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
    if (turnCount === 1) {
        let {
            firstAction,
            firstPosition
        } = firstAct(board);

        let context = new PlayerContext(firstAction, firstPosition);
        return context;
    }

    return openTo(board);
}

function firstAct(board) {
    const startup = startupArea(board.length);
    const unopeneds = startup.filter((position) => board[position] < 0);

    const seed = unopeneds.length === 0 ? board.length : unopeneds.length;
    const random = Math.random() * seed;
    const selectedIndex = Math.floor(random);

    const unopened = unopeneds[selectedIndex];
    const firstPosition = unopened;
    const firstAction = playerAction.Open;

    return {
        firstAction,
        firstPosition,
        startup
    };
}

function startupArea(boardLength) {
    const leftTop = 0;
    const rightTop = _column - 1;
    const leftBottom = _column * (_row - 1);
    const rightBottom = boardLength - 1;
    const area = [leftTop, rightTop, leftBottom, rightBottom];
    return area;
}

function MineBlock(index, state) {
    this.index = index;
    this.state = state;
}

function getArroundBlocks(index, composition) {
    const blocks = [];
    const column = index % _column;
    const row = Math.trunc(index / _column);

    const left = column - 1;
    const top = row - 1;
    const right = column + 1;
    const bottom = row + 1;

    const upperColumn = index - _column;
    const leftTop = left < 0 ? null : top < 0 ? null : upperColumn - 1;
    const midTop = top < 0 ? null : upperColumn;
    const rightTop = top < 0 ? null : right > _column - 1 ? null : upperColumn + 1;

    const leftMid = left < 0 ? null : index - 1;
    const rightMid = right > _column - 1 ? null : index + 1;

    const lowerColumn = index + _column;
    const leftBottom = left < 0 ? null : bottom > _row - 1 ? null : lowerColumn - 1;
    const midBottom = bottom > _row - 1 ? null : lowerColumn;
    const rightBottom = right > _column - 1 ? null : bottom > _row - 1 ? null : lowerColumn + 1;

    blocks[0] = leftTop !== null ? composition[leftTop] : null;
    blocks[1] = midTop !== null ? composition[midTop] : null;
    blocks[2] = rightTop !== null ? composition[rightTop] : null;
    blocks[3] = leftMid !== null ? composition[leftMid] : null;
    blocks[4] = rightMid !== null ? composition[rightMid] : null;
    blocks[5] = leftBottom !== null ? composition[leftBottom] : null;
    blocks[6] = midBottom !== null ? composition[midBottom] : null;
    blocks[7] = rightBottom !== null ? composition[rightBottom] : null;

    return blocks;
}

function openTo(board) {
    // 전체 box 획득.
    const composition = [];
    for (let i = 0; i < board.length; i++) {
        const box = board[i];
        const block = new MineBlock(i, box);

        composition[i] = block;
    }

    const targets = composition.filter((box) => box.state > 0).sort();
    const withArounds = targets.map(function (target) {
        return {
            index: target.index,
            state: target.state,
            around: getArroundBlocks(target.index, composition)
        }
    })
        .sort(function (a, b) {
            return a.around.length - b.around.length >= 0;
        });

    for (let box of withArounds) {

        const unopened = box.around.filter(a => a != null);
        const mineCount = box.state;

        // 깃발, 닫힌 박스가 현재 박스의 개수와 같으면.
        if (unopened.filter(unopen => unopen.state < 0).length === mineCount) {
            // 그 중 빈 박스를 찾아
            const selectedBox = unopened.find(unopen => unopen.state === -1);
            if (selectedBox == null) {
                // 이미 모든 깃발 마크이므로 다음 열거 진행.
                continue;
            }

            // 첫번째 박스 선택후 깃발 마크.
            return new PlayerContext(playerAction.Mark, selectedBox.index);
        }

        // 깃발, 닫힌 박스가 현재 박스보다 많을 때,            
        // 찾은 주변 박스 중 깃발 개수가 현재 개수와 같으면 open.
        if (unopened.filter(unopen => unopen.state === -2).length === mineCount) {
            const selectedBox = unopened.find(unopen => unopen.state === -1);
            if (selectedBox == null) {
                continue;
            }

            // 첫번째 박스 선택후 오픈.
            return new PlayerContext(playerAction.Open, selectedBox.index);
        }
    }

    // 타인의 깃발이 모두 옳다고 가정
    if (composition.filter(box => box.state === -2).length === _totalMineCount) {
        // 지뢰를 모두 찾음.
        return new PlayerContext(playerAction.Close, -1);
    }

    // 완전히 모르면 랜덤
    const unknowns = composition.filter(box => box.state === -1);
    const selectedIndex = Math.floor(Math.random() * (unknowns.length));
    const unknonwBox = unknowns[selectedIndex];
    return new PlayerContext(playerAction.Open, unknonwBox.index);
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