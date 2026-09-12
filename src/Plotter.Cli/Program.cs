using Microsoft.Extensions.DependencyInjection;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Composition;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.Cli.Presentation.Cli;

var (fileSelection, parsedArgs) = FileSelectionParser.Parse(args);
var arguments = parsedArgs.ToList();
var explicitFile = fileSelection.ExplicitPath;

using var provider = ServiceRegistration.Build();
var resolver = provider.GetRequiredService<INovelFileResolver>();
var store = provider.GetRequiredService<INovelWorkspaceStore>();
var file = resolver.Resolve(explicitFile);

try
{
    var command = arguments.FirstOrDefault()?.ToLowerInvariant();
    if (command is null or "help" or "--help")
    {
        PrintHelp();
        return;
    }

    if (command == "init")
    {
        await WorkspaceCommandModule.RunInitAsync(store, file);
        return;
    }

    var workspace = await store.LoadAsync(file);
    switch (command)
    {
        case "participant":
            EntityCommandModule.RunAddParticipant(workspace, arguments);
            break;
        case "location":
            if (arguments.ElementAtOrDefault(1)?.Equals("add", StringComparison.OrdinalIgnoreCase) == true)
            {
                EntityCommandModule.RunAddLocation(workspace, arguments);
                break;
            }
            LocationCommandModule.RunLocationShow(workspace, arguments);
            return;
        case "scene":
            if (arguments.ElementAtOrDefault(1)?.Equals("list", StringComparison.OrdinalIgnoreCase) == true)
            {
                TimelineCommandModule.RunSceneList(workspace, arguments);
                return;
            }
            await SceneCommandModule.RunAsync(workspace, arguments, store, file);
            return;
        case "scenes":
            if (arguments.ElementAtOrDefault(1)?.Equals("lanes", StringComparison.OrdinalIgnoreCase) == true)
            {
                CharacterCommandModule.RunLanes(workspace, arguments);
                return;
            }
            TimelineCommandModule.RunScenesTimeline(workspace, arguments);
            return;
        case "character":
            CharacterCommandModule.RunCharacter(workspace, arguments);
            return;
        case "locations":
            LocationCommandModule.RunLocationsList(workspace, arguments);
            return;
        case "threads":
            PlotThreadCommandModule.RunThreadsMatrix(workspace);
            return;
        case "thread":
            PlotThreadCommandModule.RunThreadShow(workspace, arguments);
            return;
        case "audit":
            AuditCommandModule.RunAudit(workspace, arguments);
            return;
        case "interaction":
            if (arguments.ElementAtOrDefault(1)?.Equals("add", StringComparison.OrdinalIgnoreCase) == true)
            {
                await InteractionCommandModule.RunAddAsync(workspace, arguments, store, file);
                return;
            }
            InteractionCommandModule.RunHistory(workspace, arguments);
            return;
        case "continuity":
            AuditCommandModule.RunGaps(workspace);
            return;
        case "travel":
            TravelCommandModule.RunTravel(workspace, arguments);
            return;
        case "plot":
            switch (arguments.ElementAtOrDefault(1)?.ToLowerInvariant())
            {
                case "add":
                    PlotCommandModule.RunAdd(workspace, arguments);
                    return;
                case "remove":
                    PlotCommandModule.RunRemove(workspace, arguments);
                    return;
                default:
                    PlotCommandModule.RunList(workspace);
                    return;
            }
        case "where":
            LocationCommandModule.RunWhere(workspace, arguments);
            return;
        case "lanes":
            CharacterCommandModule.RunLanes(workspace, arguments);
            return;
        case "timeline":
            TimelineCommandModule.RunTimeline(workspace, arguments);
            return;
        default:
            throw new InvalidOperationException($"Unknown command '{command}'. Run 'novel help'.");
    }

    await store.SaveAsync(file, workspace);
    Console.WriteLine($"Saved {file}");
}
catch (Exception error)
{
    Console.Error.WriteLine($"ERROR  {error.Message}");
    Environment.ExitCode = 1;
}

static void PrintHelp() => Console.WriteLine("novel init [--file PATH]\nnovel participant add NAME\nnovel location add NAME\nnovel scene add ID\nnovel scene set ID [--date-time VALUE] [--participant ID] [--location ID] [--title VALUE] [--status VALUE] [--notes VALUE]\nnovel scene list\nnovel scene show ID\nnovel timeline");