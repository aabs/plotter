using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class TimelineCommandModule
{
    public static int RunTimeline(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var order = FindOptionValue(args, "--order")?.ToLowerInvariant();
        if (order == "manuscript")
        {
            TimelineRenderers.RenderManuscript(TimelineQueries.GetManuscript(workspace));
            return 0;
        }

        var query = new SceneListQuery(
            From: FindOptionValue(args, "--from"),
            To: FindOptionValue(args, "--to"));
        TimelineRenderers.RenderChronological(workspace, TimelineQueries.GetChronological(workspace, query));
        return 0;
    }

    public static int RunScenesTimeline(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var query = new SceneListQuery(
            From: FindOptionValue(args, "--from"),
            To: FindOptionValue(args, "--to"));
        TimelineRenderers.RenderChronological(workspace, TimelineQueries.GetChronological(workspace, query));
        return 0;
    }

    public static int RunSceneList(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var order = FindOptionValue(args, "--order")?.ToLowerInvariant();
        if (order == "manuscript")
        {
            TimelineRenderers.RenderManuscript(TimelineQueries.GetManuscript(workspace));
            return 0;
        }

        var query = new SceneListQuery(
            From: FindOptionValue(args, "--from"),
            To: FindOptionValue(args, "--to"));
        TimelineRenderers.RenderChronological(workspace, TimelineQueries.GetChronological(workspace, query));
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