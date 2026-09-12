using Microsoft.Extensions.DependencyInjection;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Composition;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.Cli.Presentation.Cli;

var arguments = args.ToList();
var fileOption = arguments.IndexOf("--file");
var explicitFile = fileOption >= 0 && fileOption + 1 < arguments.Count ? arguments[fileOption + 1] : null;
if (fileOption >= 0)
    arguments.RemoveRange(fileOption, Math.Min(2, arguments.Count - fileOption));

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
            EntityCommandModule.RunAddLocation(workspace, arguments);
            break;
        case "scene":
            await SceneCommandModule.RunAsync(workspace, arguments, store, file);
            return;
        case "timeline":
            TextRenderers.RenderTimeline(workspace);
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