using Plotter.Cli.Application.Commands;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Spectre.Console;

namespace Plotter.Cli.Presentation.Cli;

public static class InteractionCommandModule
{
  public static async Task<int> RunAddAsync(NovelWorkspace workspace, IReadOnlyList<string> args, INovelWorkspaceStore store, string file, CancellationToken cancellationToken = default)
  {
    var sceneId = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel interaction add <scene-id> [--type T] [--description D] [--participant A,B]");
    var type = FindOptionValue(args, "--type");
    var description = FindOptionValue(args, "--description");
    var participantsValue = FindOptionValue(args, "--participant");
    var participants = participantsValue?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

    var result = InteractionCommands.AddInteraction(workspace, sceneId, type, description, participants);
    if (result.Success)
    {
      await store.SaveAsync(file, workspace, cancellationToken);
      AnsiConsole.MarkupLine($"[green]Added interaction[/] to {sceneId}");
    }
    return result.Success ? 0 : 1;
  }

  public static int RunHistory(NovelWorkspace workspace, IReadOnlyList<string> args)
  {
    var participant = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel interaction history <participant>");
    var result = InteractionQueries.GetParticipantHistory(workspace, participant);
    foreach (var row in result.Rows)
      Console.WriteLine($"{row.SceneId,-8} {row.Type ?? "?"}  {row.Description}");
    return 0;
  }

  private static string? FindOptionValue(IReadOnlyList<string> args, string option)
  {
    for (var index = 0; index < args.Count; index++)
      if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Count)
        return args[index + 1];
    return null;
  }
}
