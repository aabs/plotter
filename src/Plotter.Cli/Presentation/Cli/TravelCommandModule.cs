using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Travel;

namespace Plotter.Cli.Presentation.Cli;

public static class TravelCommandModule
{
    private static readonly NullTravelModel DefaultModel = new();

    public static int RunTravel(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var participant = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel travel <participant> --date YYYY-MM-DD");
        TravelRenderers.RenderTravel(workspace, TravelQueries.GetTravel(workspace, participant, DefaultModel));
        return 0;
    }
}