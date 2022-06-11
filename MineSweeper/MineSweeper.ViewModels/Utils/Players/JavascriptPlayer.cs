using Microsoft.ClearScript.V8;
using MineSweeper.Commons.Extensions;
using MineSweeper.Player;
using NLog;

namespace MineSweeper.ViewModels.Utils.Players;

public class JavascriptPlayer : IPlayer, IDisposable
{
    V8ScriptEngine _engine;

    private readonly ILogger _logger;

    public JavascriptPlayer(string jsFilePath, ILogger logger)
    {

        _engine = new V8ScriptEngine();
        var logConsole = new ConsoleLoggerMediator(logger);
        _engine.AddHostObject("console", logConsole);
        var script = File.ReadAllText(jsFilePath);
        _engine.Execute(script);

        _logger = logger;
    }

    public string GetName()
    {
        return _engine.Script.GetName();
    }

    public void Initialize(int myNumber, int column, int row, int totalMineCount)
    {
        _engine.Script.Initialize(myNumber, column, row, totalMineCount);
    }

    public PlayContext Turn(int[] board, int turnCount)
    {
        try
        {
            // ClearScript
            // board 를 그냥 넘기면 정상적인 int[] 가 넘어가지 않는다.
            var array = $"board = new Int32Array([{string.Join(",", board)}])";
            _engine.Evaluate(array);

            var turn = _engine.Script.Turn(_engine.Script.board, turnCount);
            var result = new PlayContext((PlayerAction)turn.Action, turn.Position);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }

    public void Dispose()
    {
        _engine?.Dispose();
    }
}
