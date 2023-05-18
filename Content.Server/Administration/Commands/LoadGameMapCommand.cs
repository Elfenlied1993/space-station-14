using System.Linq;
using Content.Server.GameTicking;
using Content.Server.Maps;
using Content.Shared.Administration;
using Robust.Server.Maps;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Server.Administration.Commands
{
    [AdminCommand(AdminFlags.Round | AdminFlags.Spawn)]
    public sealed class LoadGameMapCommand : IConsoleCommand
    {
        public string Command => "loadgamemap";

        public string Description => "Loads the given game map at the given coordinates.";

        public string Help => "loadgamemap <mapid> <gamemap> [<x> <y> [<name>]] ";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
            var entityManager = IoCManager.Resolve<IEntityManager>();
            var gameTicker = entityManager.EntitySysManager.GetEntitySystem<GameTicker>();

            if (args.Length is not (2 or 4 or 5))
            {
                shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }

            if (prototypeManager.TryIndex<GameMapPrototype>(args[1], out var gameMap))
            {
                if (!int.TryParse(args[0], out var mapId))
                    return;

                var loadOptions = new MapLoadOptions()
                {
                    LoadMap = false,
                };

                var stationName = args.Length == 5 ? args[4] : null;

                if (args.Length >= 4 && int.TryParse(args[2], out var x) && int.TryParse(args[3], out var y))
                {
                    loadOptions.Offset = new Vector2(x, y);
                }
                var grids = gameTicker.LoadGameMap(gameMap, new MapId(mapId), loadOptions, stationName);
                shell.WriteLine($"Loaded {grids.Count} grids.");
            }
            else
            {
                shell.WriteError($"The given map prototype {args[0]} is invalid.");
            }
        }

        public List<string> ExecuteResult(IConsoleShell shell, string argStr, string[] args)
        {
            throw new NotImplementedException();
        }

        public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
        {
            switch (args.Length)
            {
                case 1:
                    return CompletionResult.FromHint(Loc.GetString("cmd-hint-savemap-id"));
                case 2:
                    var opts = CompletionHelper.PrototypeIDs<GameMapPrototype>();
                    return CompletionResult.FromHintOptions(opts, Loc.GetString("cmd-hint-savemap-path"));
                case 3:
                    return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-x-position"));
                case 4:
                    return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-y-position"));
                case 5:
                    return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-rotation"));
                case 6:
                    return CompletionResult.FromHint(Loc.GetString("cmd-hint-loadmap-uids"));
            }

            return CompletionResult.Empty;
        }
    }

    [AdminCommand(AdminFlags.Round | AdminFlags.Spawn)]
    public sealed class ListGameMaps : IConsoleCommand
    {
        public string Command => "listgamemaps";

        public string Description => "Lists the game maps that can be used by loadgamemap";

        public string Help => "listgamemaps";

        public void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
            var entityManager = IoCManager.Resolve<IEntityManager>();
            var gameTicker = entityManager.EntitySysManager.GetEntitySystem<GameTicker>();

            if (args.Length != 0)
            {
                shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
                return;
            }
            foreach (var prototype in prototypeManager.EnumeratePrototypes<GameMapPrototype>())
            {
                shell.WriteLine($"{prototype.ID} - {prototype.MapName}");
            }
        }

        public List<string> ExecuteResult(IConsoleShell shell, string argStr, string[] args)
        {
            var prototypeManager = IoCManager.Resolve<IPrototypeManager>();
            var entityManager = IoCManager.Resolve<IEntityManager>();
            var gameTicker = entityManager.EntitySysManager.GetEntitySystem<GameTicker>();

            if (args.Length != 0)
            {
                shell.WriteError(Loc.GetString("shell-wrong-arguments-number"));
                return new List<string>();
            }

            var result = new List<string>();
            foreach (var prototype in prototypeManager.EnumeratePrototypes<GameMapPrototype>())
            {
                result.Add($"{prototype.ID} - {prototype.MapName}");
                //shell.WriteLine($"{prototype.ID} - {prototype.MapName}");
            }

            return result;
        }
    }
}
