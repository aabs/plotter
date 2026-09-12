using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class LocationCommandModule
{
    public static int RunLocationShow(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var location = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel location show <location> [--timeline]");
        LocationRenderers.RenderLocationTimeline(workspace, LocationQueries.GetLocationTimeline(workspace, location));
        return 0;
    }

    public static int RunLocationsList(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        if (!args.Any(argument => argument.Equals("--occupancy", StringComparison.OrdinalIgnoreCase)))
            throw new ArgumentException("Usage: novel locations list --occupancy");
        LocationRenderers.RenderLocationDensity(workspace, LocationQueries.GetLocationDensity(workspace));
        return 0;
    }

    public static int RunWhere(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var atValue = FindOptionValue(args, "--at") ?? throw new ArgumentException("Usage: novel where [participant] --at YYYY-MM-DDTHH:mm");
        var at = DateTime.Parse(atValue, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        var participant = args.ElementAtOrDefault(1);
        if (participant is not null && !participant.StartsWith("--", StringComparison.Ordinal))
        {
            LocationRenderers.RenderWhere(workspace, LocationQueries.GetActiveLocation(workspace, participant, at));
            return 0;
        }

        foreach (var id in workspace.Participants.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))
            LocationRenderers.RenderWhere(workspace, LocationQueries.GetActiveLocation(workspace, id, at));
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
