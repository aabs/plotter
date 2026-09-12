using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Composition;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.Cli.Infrastructure.Storage;

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
        await store.SaveAsync(file, new NovelWorkspace());
        Console.WriteLine($"Initialized {file}");
        return;
    }

    var workspace = await store.LoadAsync(file);
    switch (command)
    {
        case "participant":
            AddParticipant(workspace, arguments);
            break;
        case "location":
            AddLocation(workspace, arguments);
            break;
        case "scene":
            await HandleSceneAsync(workspace, arguments, store, file);
            return;
        case "timeline":
            PrintTimeline(workspace);
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

static void AddParticipant(NovelWorkspace workspace, IReadOnlyList<string> args)
{
    var id = new ParticipantId(Validation.RequiredId(args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel participant add <name>"), "Participant name"));
    Validation.EnsureUnique(workspace.Participants, id.Value, "Participant");
    workspace.Participants[id.Value] = new Participant(id, id.Value);
}

static void AddLocation(NovelWorkspace workspace, IReadOnlyList<string> args)
{
    var id = new LocationId(Validation.RequiredId(args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel location add <name>"), "Location name"));
    Validation.EnsureUnique(workspace.Locations, id.Value, "Location");
    workspace.Locations[id.Value] = new Location(id, id.Value);
}

static async Task HandleSceneAsync(NovelWorkspace workspace, IReadOnlyList<string> args, INovelWorkspaceStore store, string file)
{
    var operation = args.ElementAtOrDefault(1)?.ToLowerInvariant();
    if (operation == "add")
    {
        var id = new SceneId(Validation.RequiredId(args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene add <scene-id>"), "Scene ID"));
        Validation.EnsureUnique(workspace.Scenes, id.Value, "Scene");
        workspace.Scenes[id.Value] = new Scene(id, id.Value);
        await store.SaveAsync(file, workspace);
        Console.WriteLine($"Created scene {id.Value}");
        return;
    }

    if (operation == "set")
    {
        var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene set <scene-id> [options]");
        if (!workspace.Scenes.TryGetValue(id, out var scene))
            throw new InvalidOperationException($"Scene '{id}' does not exist.");

        var dateIndex = FindOption(args, "--date-time");
        var participantIndex = FindOption(args, "--participant");
        var locationIndex = FindOption(args, "--location");

        var story = dateIndex >= 0
            ? new StoryTime(DateTime.Parse(args[dateIndex + 1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind))
            : scene.StoryTime;
        var participantIds = participantIndex >= 0
            ? scene.ParticipantIds.Append(new ParticipantId(args[participantIndex + 1])).Distinct().ToArray()
            : scene.ParticipantIds;
        var locationId = locationIndex >= 0 ? new LocationId(args[locationIndex + 1]) : scene.LocationId;

        var updated = scene with { StoryTime = story, ParticipantIds = participantIds, LocationId = locationId };
        workspace.Scenes[id] = updated;
        await store.SaveAsync(file, workspace);
        Console.WriteLine($"Updated scene {id}");
        return;
    }

    if (operation is "list" or "show")
    {
        if (operation == "show")
        {
            var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene show <scene-id>");
            if (!workspace.Scenes.TryGetValue(id, out var scene))
                throw new InvalidOperationException($"Scene '{id}' does not exist.");
            PrintScene(workspace, scene);
        }
        else
        {
            foreach (var scene in workspace.Scenes.Values.OrderBy(s => s.StoryTime?.Date ?? DateTime.MaxValue).ThenBy(s => s.Id.Value, StringComparer.OrdinalIgnoreCase))
                PrintScene(workspace, scene, compact: true);
        }
        return;
    }

    throw new ArgumentException("Usage: novel scene add|set|list|show ...");
}

static int FindOption(IReadOnlyList<string> args, string option)
{
    for (var index = 0; index < args.Count; index++)
        if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase))
            return index;
    return -1;
}

static void PrintTimeline(NovelWorkspace workspace)
{
    foreach (var scene in workspace.Scenes.Values.OrderBy(s => s.StoryTime?.Date ?? DateTime.MaxValue).ThenBy(s => s.Id.Value, StringComparer.OrdinalIgnoreCase))
        PrintScene(workspace, scene, true);
}

static void PrintScene(NovelWorkspace workspace, Scene scene, bool compact = false)
{
    var date = scene.StoryTime?.Date?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? "unknown time";
    var location = scene.LocationId is null ? "" : workspace.Locations.GetValueOrDefault(scene.LocationId.Value.Value)?.Name ?? scene.LocationId.Value.Value;
    var participants = string.Join(", ", scene.ParticipantIds.Select(id => workspace.Participants.GetValueOrDefault(id.Value)?.Name ?? id.Value));
    if (compact)
    {
        Console.WriteLine($"{date,16}  {scene.Id.Value,-8} {scene.Title}  {location} · {participants}");
        return;
    }
    Console.WriteLine($"{scene.Id.Value} · {scene.Title}\nStory time: {date}\nLocation:   {location}\nParticipants: {participants}\n");
}

static void PrintHelp() => Console.WriteLine("novel init [--file PATH]\nnovel participant add NAME\nnovel location add NAME\nnovel scene add ID\nnovel scene set ID [--date-time VALUE] [--participant ID] [--location ID]\nnovel scene list\nnovel scene show ID\nnovel timeline");