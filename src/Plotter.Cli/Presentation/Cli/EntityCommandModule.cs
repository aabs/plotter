using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Spectre.Console;

namespace Plotter.Cli.Presentation.Cli;

public static class EntityCommandModule
{
    public static int RunAddParticipant(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var name = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel participant add <name>");
        var result = EntityCommands.AddParticipant(workspace, name);
        if (result.Success)
            AnsiConsole.MarkupLine($"[green]Added participant[/] {name}");
        return result.Success ? 0 : 1;
    }

    public static int RunAddLocation(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var name = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel location add <name>");
        var result = EntityCommands.AddLocation(workspace, name);
        if (result.Success)
            AnsiConsole.MarkupLine($"[green]Added location[/] {name}");
        return result.Success ? 0 : 1;
    }
}