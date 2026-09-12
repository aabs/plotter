using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class SceneDetailCommandModule
{
    public static int RunShow(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel scene show <scene-id>");
        var card = SceneDetailQueries.GetSceneDetail(workspace, id);
        if (card is null)
            throw new InvalidOperationException($"Scene '{id}' does not exist.");
        SceneCardRenderer.RenderCard(workspace, card);
        return 0;
    }
}