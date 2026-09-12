using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class LocationRenderers
{
    public static void RenderLocationTimeline(NovelWorkspace workspace, LocationTimelineResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        var name = workspace.Locations.GetValueOrDefault(result.LocationId)?.Name ?? result.LocationId;
        writer.WriteLine(name);
        writer.WriteLine();
        foreach (var row in result.Rows)
        {
            var range = row.End is { } end
                ? $"{row.Start:HH:mm}–{end:HH:mm}"
                : $"{row.Start:HH:mm}";
            writer.WriteLine($"{range,-14} {row.SceneId,-6} {row.Title}");
        }
    }

    public static void RenderLocationDensity(NovelWorkspace workspace, LocationDensityResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        writer.WriteLine($"{"Location",-20} {"Scenes",-6} {"First use",-12} {"Last use",-12}");
        writer.WriteLine($"{new string('-', 20)} {new string('-', 6)} {new string('-', 12)} {new string('-', 12)}");
        foreach (var row in result.Rows)
        {
            var name = workspace.Locations.GetValueOrDefault(row.LocationId)?.Name ?? row.LocationId;
            var first = row.FirstUse?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—";
            var last = row.LastUse?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "—";
            writer.WriteLine($"{name,-20} {row.SceneCount,-6} {first,-12} {last,-12}");
        }
    }

    public static void RenderWhere(NovelWorkspace workspace, WhereResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        var name = workspace.Participants.GetValueOrDefault(result.ParticipantId)?.Name ?? result.ParticipantId;
        if (result.LocationId is not null)
            writer.WriteLine($"{name} is at {LocationName(workspace, result.LocationId)}.");
        else if (result.ConflictLocationIds.Count > 0)
            writer.WriteLine($"{name} is in conflict: multiple locations {string.Join(", ", result.ConflictLocationIds.Select(id => LocationName(workspace, id)))}.");
        else
            writer.WriteLine($"{name} has no recorded location.");
    }

    private static string LocationName(NovelWorkspace workspace, string locationId) =>
        workspace.Locations.GetValueOrDefault(locationId)?.Name ?? locationId;
}
