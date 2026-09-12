using System.Globalization;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.Cli.Infrastructure.Storage;

var arguments = args.ToList();
var fileOption = arguments.IndexOf("--file");
var file = NovelFileResolver.Resolve(fileOption >= 0 && fileOption + 1 < arguments.Count ? arguments[fileOption + 1] : null);
if (fileOption >= 0) arguments.RemoveRange(fileOption, Math.Min(2, arguments.Count - fileOption));

try
{
    var command = arguments.FirstOrDefault()?.ToLowerInvariant();
    if (command is null or "help" or "--help") { PrintHelp(); return; }
    if (command == "init") { await TomlWorkspaceStore.SaveAsync(file, new NovelWorkspace()); Console.WriteLine($"Initialized {file}"); return; }
    var workspace = await TomlWorkspaceStore.LoadAsync(file);
    switch (command)
    {
        case "participant": AddParticipant(workspace, arguments); break;
        case "location": AddLocation(workspace, arguments); break;
        case "scene": await HandleSceneAsync(workspace, arguments, file); return;
        case "timeline": PrintTimeline(workspace); return;
        default: throw new InvalidOperationException($"Unknown command '{command}'. Run 'novel help'.");
    }
    await TomlWorkspaceStore.SaveAsync(file, workspace);
    Console.WriteLine($"Saved {file}");
}
catch (Exception error)
{
    Console.Error.WriteLine($"ERROR  {error.Message}");
    Environment.ExitCode = 1;
}

static void AddParticipant(NovelWorkspace workspace, IReadOnlyList<string> args)
{
    var id = Validation.RequiredId(args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel participant add <name>"), "Participant name");
    Validation.EnsureUnique(workspace.Participants, id, "Participant");
    workspace.Participants[id] = new Participant(id, id);
}

static void AddLocation(NovelWorkspace workspace, IReadOnlyList<string> args)
{
    var id = Validation.RequiredId(args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel location add <name>"), "Location name");
    Validation.EnsureUnique(workspace.Locations, id, "Location");
    workspace.Locations[id] = new Location(id, id);
}

static async Task HandleSceneAsync(NovelWorkspace workspace, IReadOnlyList<string> args, string file)
{
    var operation = args.ElementAtOrDefault(1)?.ToLowerInvariant();
    if (operation == "add")
    {
        var id = Validation.RequiredId(args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene add <scene-id>"), "Scene ID");
        Validation.EnsureUnique(workspace.Scenes, id, "Scene");
        workspace.Scenes[id] = new Scene(id, id);
        await TomlWorkspaceStore.SaveAsync(file, workspace);
        Console.WriteLine($"Created scene {id}");
        return;
    }
    if (operation == "set")
    {
        var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene set <scene-id> [options]");
        if (!workspace.Scenes.TryGetValue(id, out var scene)) throw new InvalidOperationException($"Scene '{id}' does not exist.");
        var dateIndex = FindOption(args, "--date-time");
        var participantIndex = FindOption(args, "--participant");
        var locationIndex = FindOption(args, "--location");
        var date = dateIndex >= 0 ? DateTime.Parse(args[dateIndex + 1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) : scene.StoryDateTime;
        var participantIds = participantIndex >= 0 ? scene.ParticipantIds.Append(args[participantIndex + 1]).Distinct(StringComparer.OrdinalIgnoreCase).ToArray() : scene.ParticipantIds;
        var locationId = locationIndex >= 0 ? args[locationIndex + 1] : scene.LocationId;
        var updated = scene with { StoryDateTime = date, ParticipantIds = participantIds, LocationId = locationId };
        Validation.EnsureSceneReferences(workspace, updated);
        workspace.Scenes[id] = updated;
        await TomlWorkspaceStore.SaveAsync(file, workspace);
        Console.WriteLine($"Updated scene {id}");
        return;
    }
    if (operation is "list" or "show")
    {
        if (operation == "show")
        {
            var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene show <scene-id>");
            if (!workspace.Scenes.TryGetValue(id, out var scene)) throw new InvalidOperationException($"Scene '{id}' does not exist.");
            PrintScene(workspace, scene);
        }
        else foreach (var scene in workspace.Scenes.Values.OrderBy(s => s.StoryDateTime ?? DateTime.MaxValue).ThenBy(s => s.Id)) PrintScene(workspace, scene, compact: true);
        return;
    }
    throw new ArgumentException("Usage: novel scene add|set|list|show ...");
}

static int FindOption(IReadOnlyList<string> args, string option)
{
    for (var index = 0; index < args.Count; index++)
        if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase)) return index;
    return -1;
}

static void PrintTimeline(NovelWorkspace workspace)
{
    foreach (var scene in workspace.Scenes.Values.OrderBy(s => s.StoryDateTime ?? DateTime.MaxValue).ThenBy(s => s.Id))
        PrintScene(workspace, scene, true);
}

static void PrintScene(NovelWorkspace workspace, Scene scene, bool compact = false)
{
    var date = scene.StoryDateTime?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? "unknown time";
    var location = scene.LocationId is null ? "" : workspace.Locations.GetValueOrDefault(scene.LocationId)?.Name ?? scene.LocationId;
    var participants = string.Join(", ", scene.ParticipantIds.Select(id => workspace.Participants.GetValueOrDefault(id)?.Name ?? id));
    if (compact) { Console.WriteLine($"{date,16}  {scene.Id,-8} {scene.Title}  {location} · {participants}"); return; }
    Console.WriteLine($"{scene.Id} · {scene.Title}\nStory time: {date}\nLocation:   {location}\nParticipants: {participants}\n");
}

static void PrintHelp() => Console.WriteLine("novel init [--file PATH]\nnovel participant add NAME\nnovel location add NAME\nnovel scene add ID\nnovel scene set ID [--date-time VALUE] [--participant ID] [--location ID]\nnovel scene list\nnovel scene show ID\nnovel timeline");
