using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class PlotThreadCommandModule
{
    public static int RunThreadsMatrix(NovelWorkspace workspace)
    {
        PlotThreadRenderers.RenderThreadMatrix(PlotThreadQueries.GetThreadMatrix(workspace));
        return 0;
    }

    public static int RunThreadShow(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var plot = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel thread show <plot-thread>");
        PlotThreadRenderers.RenderThreadTimeline(PlotThreadQueries.GetThreadTimeline(workspace, plot));
        return 0;
    }
}