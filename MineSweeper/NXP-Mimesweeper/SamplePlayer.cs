using MineSweeper.Player;

namespace NXP.CSharp.MineSweeper;

public class SamplePlayer
{
    // sample!

    private int _myNumber;

    private int _column;

    private int _row;

    private int _totalMineCount;

    public void Initialize(int myNumber, int column, int row, int totalMineCount)
    {
        _myNumber = myNumber;
        _column = column;
        _row = row;
        _totalMineCount = totalMineCount;
    }

    public PlayContext Turn(int[] board, int turnCount)
    {
        if (turnCount is 1)
        {
            var (firstAction, firstPosition) = FirstAct(board);
            if (firstPosition is -1) // 이미 첫턴에서 금지구역이 다 열렸으므로 firstAct 에서 선택할 것이 없다.
            {
                return OpenTo(board);
            }

            return new PlayContext(firstAction, firstPosition);
        }

        return OpenTo(board);

        //return MarkTo(board);
    }

    private PlayContext OpenTo(int[] board)
    {
        try
        {
            // 전체 box 획득.
            var composition = new MineBlock[board.Length];
            for (int i = 0; i < board.Length; i++)
            {
                var box = board[i];
                var block = new MineBlock
                {
                    Index = i,
                    State = box
                };
                composition[i] = block;
            }

            // 가장 작은 number 를 가진 box 순서 정렬
            var targets = composition.Where(box => box.State > 0).OrderBy(box => box.State).ToList();

            // 주변 8칸 중 열리지 않은 박스가 가장 적은 순서대로 다시 정렬.
            var withArrounds = targets.Select(target => new
            {
                target.Index,
                Status = target.State,
                Arround = GetArroundBlocks(target.Index, composition)
            }).OrderBy(blocks => blocks.Arround.Length).ToList();

            foreach (var box in withArrounds)
            {
                var unopened = box.Arround;
                var mineCount = box.Status;

                // 깃발, 닫힌 박스가 현재 박스의 개수와 같으면.
                if (unopened.Where(unopen => unopen?.State < 0).Count() == mineCount)
                {
                    // 그 중 빈 박스를 찾아
                    var selectedBox = unopened.FirstOrDefault(unopen => unopen?.State is -1);
                    if (selectedBox is null)
                    {
                        // 이미 모든 깃발 마크이므로 다음 열거 진행.
                        continue;
                    }

                    // 첫번째 박스 선택후 깃발 마크.
                    return new PlayContext(PlayerAction.Mark, selectedBox.Index);
                }

                // 깃발, 닫힌 박스가 현재 박스보다 많을 때,            
                // 찾은 주변 박스 중 깃발 개수가 현재 개수와 같으면 open.
                if (unopened.Where(unopen => unopen?.State is -2).Count() == mineCount)
                {
                    var selectedBox = unopened.FirstOrDefault(unopen => unopen?.State is -1);
                    if (selectedBox is null)
                    {
                        continue;
                    }

                    // 첫번째 박스 선택후 깃발 마크.
                    return new PlayContext(PlayerAction.Open, selectedBox.Index);
                }
            }

            // 타인의 깃발이 모두 옳다고 가정
            if (composition.Count(box => box.State is -2) == _totalMineCount)
            {
                // 지뢰를 모두 찾음.
                return new PlayContext(PlayerAction.Close, -1);
            }

            // 완전히 모르면 랜덤
            var unknowns = composition.Where(box => box.State is -1).ToList();
            var selectedIndex = new Random().Next(0, unknowns.Count - 1);
            var unknonwBox = unknowns[selectedIndex];
            return new PlayContext(PlayerAction.Open, unknonwBox.Index);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    class MineBlock
    {
        public int Index { get; set; }

        public int State { get; set; }
    }

    private MineBlock?[] GetArroundBlocks(int index, MineBlock[] composition)
    {
        try
        {
            var blocks = new MineBlock?[8];
            var column = index % _column;
            var row = index / _column;

            var left = column - 1;
            var top = row - 1;
            var right = column + 1;
            var bottom = row + 1;

            var upperLine = index - _column;

            int? leftTop = left < 0 ? null : top < 0 ? null : upperLine - 1;
            int? midTop = top < 0 ? null : upperLine;
            int? rightTop = top < 0 ? null : right > _column - 1 ? null : upperLine + 1;

            int? leftMid = left < 0 ? null : index - 1;
            int? rightMid = right > _column - 1 ? null : index + 1;

            var lowerLine = index + _column;

            int? leftBottom = left < 0 ? null : bottom > _row - 1 ? null : lowerLine - 1;
            int? midBottom = bottom > _row - 1 ? null : lowerLine;
            int? rightBottom = right > _column - 1 ? null : bottom > _row - 1 ? null : lowerLine + 1;

            blocks[0] = leftTop is not null ? composition[leftTop.Value] : null;
            blocks[1] = midTop is not null ? composition[midTop.Value] : null;
            blocks[2] = rightTop is not null ? composition[rightTop.Value] : null;
            blocks[3] = leftMid is not null ? composition[leftMid.Value] : null;
            blocks[4] = rightMid is not null ? composition[rightMid.Value] : null;
            blocks[5] = leftBottom is not null ? composition[leftBottom.Value] : null;
            blocks[6] = midBottom is not null ? composition[midBottom.Value] : null;
            blocks[7] = rightBottom is not null ? composition[rightBottom.Value] : null;

            return blocks;
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    // TODO : test 후 삭제.
    private PlayContext MarkTo(int[] board)
    {
        var action = PlayerAction.Mark;
        var unopeneds = new List<int>();
        for (var i = 0; i < board.Length; i++)
        {
            var box = board[i];
            if (box is -1)
            {
                unopeneds.Add(i);
            }
        }

        if (unopeneds.Count is 0)
        {
            action = PlayerAction.Close;
            return new PlayContext(action, 0);
        }

        int position;

        do
        {
            var selectedIndex = new Random().Next(0, unopeneds.Count - 1);
            var unopened = unopeneds[selectedIndex];
            position = unopened;

            // double check.
            var inBoard = board[position];
            if (inBoard is not -1)
            {
                continue;
            }

            break;
        }
        while (true);

        return new PlayContext(action, position);
    }

    // TODO : test 후 삭제.
    private (PlayerAction firstAction, int firstPosition) FirstAct(int[] board)
    {
        var startupArea = StartupArea(board.Length);
        var unopeneds = startupArea.Where(position => board[position] is -1).ToList();
        
        if (unopeneds.Count is 0)
        {
            return (PlayerAction.Open, -1);
        }

        var seed = unopeneds.Count;
        var selectedIndex = new Random().Next(0, seed - 1);
        var unopened = unopeneds[selectedIndex];
        var position = unopened;

        return (PlayerAction.Open, position);
    }

    private int[] StartupArea(int boardLength)
    {
        var leftTop = 0;
        var rightTop = _column - 1;
        var leftBottom = _column * (_row - 1);
        var rightBottom = boardLength - 1;

        var area = new int[] { leftTop, rightTop, leftBottom, rightBottom };
        return area;
    }
}
