// sample

var _myNumber;
var _column;
var _row;

function Initialize(myNumber, column, row) {
    _myNumber = myNumber;
    _column = column;
    _row = row;
}

function Turn(board, turnCount, myScore) {
    let action = Math.random() * 2;
    let position = Math.random() * (board.length - 1);
    return {
        action,
        position
    }
}