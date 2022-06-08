using MineSweeper.Constants;
using MineSweeper.Contracts;
using MineSweeper.Models;
using MineSweeper.Player;
using MineSweeper.Utils.Players.JS;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace MineSweeper.Utils.Players;

public class PlayerLoader : IPlayerLoader
{
    private List<AssemblyLoadContext> _loadAssemblies = new List<AssemblyLoadContext>(4);

    private Type playerInterface = typeof(IPlayer);

    private readonly ILogger _logger;

    public PlayerLoader(ILogger logger) => _logger = logger;

    public IEnumerable<IPlayer> LoadPlayers()
    {
        FolderCheck();

        _loadAssemblies.ForEach(asm => asm.Unload());
        _loadAssemblies.Clear();

        var root = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location);

        // TODO : 최대 4 명 로딩 체크.
        var players = new List<IPlayer>(4);
        //var players = new List<IPlayer>
        //{
        //    new SamplePlayer(),
        //    new SamplePlayer(),
        //    new SamplePlayer(),
        //    new SamplePlayer(),
        //};

        //return players;

        // load players
        // load c#
        LoadCSharpPlayer(players, root);

        // load javascript
        LoadJavaScriptPlayer(players, root);

        return players;
    }

    private void LoadJavaScriptPlayer(List<IPlayer> players, string? root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentNullException(nameof(root));
        }

        var path = Path.Combine(Strings.Players, Enum.GetName(Platform.Javascript)!);
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            var scriptPath = Path.Combine(root, file);
            var player = new JavascriptPlayer(scriptPath, _logger);
            players.Add(player);
        }
    }

    private void LoadCSharpPlayer(List<IPlayer> players, string? root)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentNullException(nameof(root));
        }

        var path = Path.Combine(Strings.Players, Enum.GetName(Platform.CS)!);
        var files = Directory.GetFiles(path);

        foreach (var file in files)
        {
            var loadContext = new AssemblyLoadContext(Guid.NewGuid().ToString(), true);
            _loadAssemblies.Add(loadContext);

            var assemblyPath = Path.Combine(root, file);
            var assembly = loadContext.LoadFromAssemblyPath(assemblyPath);

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
}
