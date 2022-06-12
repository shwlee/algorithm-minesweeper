using MineSweeper.Defines.Enums;
using MineSweeper.Defines.Utils;
using MineSweeper.Player;
using MineSweeper.ViewModels.Constants;
using MineSweeper.ViewModels.Utils.Players;
using NLog;
using System.Reflection;
using System.Runtime.Loader;

namespace MineSweeper.Utils.Players;

public class PlayerLoader : IPlayerLoader
{
    private List<AssemblyLoadContext> _loadAssemblies = new(4);

    private Type playerInterface = typeof(IPlayer);

    private IFileDialogService _fileDialog;

    private readonly ILogger _logger;

    public PlayerLoader(IFileDialogService fileDialog, ILogger logger)
    {
        _logger = logger;
        _fileDialog = fileDialog;
    }

    /// <summary>
    /// 파일 플랫폼을 선택하여 로딩.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IPlayer> LoadPlayers(Platform platform)
    {
        var players = new List<IPlayer>(4);

        // file dialog
        var files = _fileDialog.GetFiles(platform);
        switch (platform)
        {
            case Platform.CS:
                LoadSelectedCSharpPlayers(players, files!);
                break;
            case Platform.CPP:
                break;
            case Platform.Javascript:
                LoadSelectedJavascriptFiles(players, files!);
                break;
            case Platform.Python:
                break;
        }

        return players;
    }

    /// <summary>
    /// TEST. 정해진 경로에서 자동 로딩.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IPlayer> LoadAllPlayers()
    {
        // test용도에 가깝다. 실제로는 Platform 을 정해서 로딩한다.
        FolderCheck();

        _loadAssemblies.ForEach(asm => asm.Unload());
        _loadAssemblies.Clear();

        var root = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);

        // TODO : 최대 4 명 로딩 체크.
        var players = new List<IPlayer>(4);

        // load players
        // load c#
        LoadCSharpPlayer(players, root);

        // load javascript
        LoadJavaScriptPlayer(players, root);

        // load c++

        // load python

        return players;
    }

    private void LoadJavaScriptPlayer(List<IPlayer> players, string? root)
    {
        if (players.Count >= 4)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentNullException(nameof(root));
        }

        var path = Path.Combine(root, Strings.Players, Enum.GetName(Platform.Javascript)!);
        var files = Directory.GetFiles(path, "*.js");
        LoadSelectedJavascriptFiles(players, files);
    }

    private void LoadSelectedJavascriptFiles(List<IPlayer> players, string[] files)
    {
        foreach (var file in files)
        {
            if (players.Count >= 4)
            {
                return;
            }

            var player = new JavascriptPlayer(file, _logger);
            players.Add(player);
        }
    }

    private void LoadCSharpPlayer(List<IPlayer> players, string? root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentNullException(nameof(root));
        }

        var path = Path.Combine(root, Strings.Players, Enum.GetName(Platform.CS)!);
        var files = Directory.GetFiles(path);

        if (players.Count >= 4)
        {
            return;
        }

        LoadSelectedCSharpPlayers(players, files);
    }

    private void LoadSelectedCSharpPlayers(List<IPlayer> players, string[] files)
    {
        foreach (var file in files)
        {

            if (players.Count >= 4)
            {
                return;
            }

            var loadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
            _loadAssemblies.Add(loadContext);

            var assembly = loadContext.LoadFromAssemblyPath(file);

            Type? playerType = null;
            var types = assembly.GetExportedTypes();
            foreach (var type in types)
            {
                if (type.IsInterface || type.IsAbstract)
                {
                    continue;
                }

                if (playerInterface.IsAssignableFrom(type))
                {
                    playerType = type;
                    break;
                }
            }

            if (playerType is null)
            {
                continue;
            }

            if (Activator.CreateInstance(playerType) is not IPlayer player)
            {
                continue;
            }

            players.Add(player);
        }
    }

    private void FolderCheck()
    {
        // folder check
        if (Directory.Exists(Strings.Players) is false)
        {
            Directory.CreateDirectory(Strings.Players);
        }

        foreach (var platform in Enum.GetNames<Platform>())
        {
            var path = Path.Combine(Strings.Players, platform);
            if (Directory.Exists(path) is false)
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    public void ClearLoadedPlayers()
    {
        _loadAssemblies.ForEach(asm => asm.Unload());
        _loadAssemblies.Clear();
    }
}
