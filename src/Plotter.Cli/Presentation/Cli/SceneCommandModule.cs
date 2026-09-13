using System.Globalization;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Spectre.Console;

namespace Plotter.Cli.Presentation.Cli;

public static class SceneCommandModule
{
  public static async Task<int> RunAsync(NovelWorkspace workspace, IReadOnlyList<string> args, INovelWorkspaceStore store, string file, CancellationToken cancellationToken = default)
  {
    var operation = args.ElementAtOrDefault(1)?.ToLowerInvariant();
    switch (operation)
    {
      case "add":
        {
          var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene add <scene-id>");
          var result = SceneCommands.AddScene(workspace, id);
          if (result.Success)
          {
            await store.SaveAsync(file, workspace, cancellationToken);
            AnsiConsole.MarkupLine($"[green]Created scene[/] {id}");
          }
          return result.Success ? 0 : 1;
        }
      case "set":
        {
          var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene set <scene-id> [options]");
          var update = ParseUpdate(args);
          var result = SceneCommands.SetScene(workspace, id, update);
          if (result.Success)
          {
            await store.SaveAsync(file, workspace, cancellationToken);
            AnsiConsole.MarkupLine($"[green]Updated scene[/] {id}");
          }
          return result.Success ? 0 : 1;
        }
      case "list":
        TextRenderers.RenderTimeline(workspace);
        return 0;
      case "show":
        return SceneDetailCommandModule.RunShow(workspace, args);
      default:
        throw new ArgumentException("Usage: novel scene add|set|list|show ...");
    }
  }

  private static SceneUpdate ParseUpdate(IReadOnlyList<string> args)
  {
    var dateIndex = FindOption(args, "--date-time");
    var participantIndex = FindOption(args, "--participant");
    var locationIndex = FindOption(args, "--location");
    var titleIndex = FindOption(args, "--title");
    var statusIndex = FindOption(args, "--status");
    var notesIndex = FindOption(args, "--notes");

    return new SceneUpdate(
        Title: titleIndex >= 0 ? args[titleIndex + 1] : null,
        StoryTime: dateIndex >= 0 ? new StoryTime(DateTime.Parse(args[dateIndex + 1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)) : null,
        LocationId: locationIndex >= 0 ? new LocationId(args[locationIndex + 1]) : null,
        ParticipantIds: participantIndex >= 0 ? [new ParticipantId(args[participantIndex + 1])] : null,
        Status: statusIndex >= 0 ? args[statusIndex + 1] : null,
        Notes: notesIndex >= 0 ? args[notesIndex + 1] : null);
  }

  private static int FindOption(IReadOnlyList<string> args, string option)
  {
    for (var index = 0; index < args.Count; index++)
      if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase))
        return index;
    return -1;
  }
}
