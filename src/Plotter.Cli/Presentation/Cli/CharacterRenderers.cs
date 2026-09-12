using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class CharacterRenderers
{
    public static void RenderItinerary(NovelWorkspace workspace, ItineraryResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        var name = workspace.Participants.GetValueOrDefault(result.ParticipantId)?.Name ?? result.ParticipantId;
        writer.WriteLine($"{name} — chronological itinerary");
        writer.WriteLine();
        foreach (var row in result.Rows)
        {
            var time = row.Time?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? "??:??";
            var location = row.LocationId is null ? "" : workspace.Locations.GetValueOrDefault(row.LocationId)?.Name ?? row.LocationId;
            writer.WriteLine($"{time}  {location,-18} {row.SceneId,-6} {row.Title}");
        }
    }

    public static void RenderLanes(NovelWorkspace workspace, LaneResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        var headers = result.ParticipantIds.Select(id => workspace.Participants.GetValueOrDefault(id)?.Name ?? id).ToArray();
        var widths = headers.Select(header => Math.Max(header.Length, 8)).ToArray();

        writer.Write("Time".PadRight(6));
        for (var index = 0; index < headers.Length; index++)
            writer.Write("  " + headers[index].PadRight(widths[index]));
        writer.WriteLine();

        writer.Write("-----");
        for (var index = 0; index < headers.Length; index++)
            writer.Write("  " + new string('-', widths[index]));
        writer.WriteLine();

        foreach (var row in result.Rows)
        {
            var time = row.Time?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? "??:??";
            writer.Write(time.PadRight(6));
            for (var index = 0; index < row.Cells.Count; index++)
            {
                var cell = row.Cells[index];
                var text = cell.Occurrences.Count == 0
                    ? "—"
                    : string.Join("; ", cell.Occurrences.Select(occurrence => $"{occurrence.SceneId} {LocationName(workspace, occurrence.LocationId)}"));
                writer.Write("  " + text.PadRight(widths[index]));
            }
            writer.WriteLine();
        }
    }

    private static string LocationName(NovelWorkspace workspace, string? locationId) =>
        locationId is null ? "" : workspace.Locations.GetValueOrDefault(locationId)?.Name ?? locationId;
}
