using MineSweeper.Constants;
using MineSweeper.Contracts;
using MineSweeper.Models;
using MineSweeper.Player;
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

    public IEnumerable<IPlayer> LoadPlayers()
    {
        FolderCheck();

        _loadAssemblies.ForEach(asm => asm.Unload());
        _loadAssemblies.Clear();

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
        var path = Path.Combine(Strings.Players, Enum.GetName<Platform>(Platform.CS)!);
        var files = Directory.GetFiles(path);
        var root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

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

        return players;
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
