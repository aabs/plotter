using Plotter.Cli.Application.Commands;
using Plotter.Cli.Infrastructure.Storage;
using Spectre.Console;

namespace Plotter.Cli.Presentation.Cli;

public static class WorkspaceCommandModule
{
    public static async Task<int> RunInitAsync(INovelWorkspaceStore store, string file, CancellationToken cancellationToken = default)
    {
        var result = await new WorkspaceCommands(store).InitAsync(file, cancellationToken);
        if (result.Success)
            AnsiConsole.MarkupLine($"[green]Initialized[/] {file}");
        return result.Success ? 0 : 1;
    }
}
