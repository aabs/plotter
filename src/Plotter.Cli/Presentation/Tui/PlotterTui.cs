using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.Cli.Presentation.Cli;
using Spectre.Console;

namespace Plotter.Cli.Presentation.Tui;

public static class PlotterTui
{
    public static async Task<int> RunAsync(NovelWorkspace workspace, INovelWorkspaceStore store, string file, CancellationToken cancellationToken = default)
    {
        var state = new TuiState();

        while (true)
        {
            AnsiConsole.Clear();
            var sceneIds = TuiQueryAdapter.SceneIdsForView(workspace, state.CurrentView, state.Filter, state.Ordering);
            state.PreserveOrFallback(sceneIds);

            if (sceneIds.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No scenes match the current view.[/]");
                break;
            }

            var selection = new SelectionPrompt<string>()
                .Title($"[bold]Scenes: {state.CurrentView}[/] — choose a scene (j/k or arrows, Enter to open, e to edit, q to quit)")
                .PageSize(10)
                .AddChoices(sceneIds.OrderBy(id => id, StringComparer.OrdinalIgnoreCase));
            var chosen = AnsiConsole.Prompt(selection);
            state.Select(chosen);

            var action = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"[bold]{chosen}[/]")
                .AddChoices("details", "edit", "quit"));
            switch (action)
            {
                case "details":
                    if (SceneDetailQueries.GetSceneDetail(workspace, chosen) is { } card)
                    {
                        AnsiConsole.Clear();
                        SceneCardRenderer.RenderCard(workspace, card);
                        AnsiConsole.WriteLine("Press any key to return...");
                        Console.ReadKey(true);
                    }
                    break;
                case "edit":
                    AnsiConsole.MarkupLine($"[cyan]Editing {chosen}[/]");
                    await store.SaveAsync(file, workspace, cancellationToken);
                    AnsiConsole.WriteLine("Press any key to return...");
                    Console.ReadKey(true);
                    break;
                default:
                    return 0;
            }
        }

        return 0;
    }
}
