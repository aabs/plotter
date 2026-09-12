using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class SceneCardRenderer
{
    public static void RenderCard(NovelWorkspace workspace, SceneCardDetail card, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        var title = $"{card.SceneId.Value} · {card.Title}";
        writer.WriteLine(title);
        writer.WriteLine(new string('─', title.Length));

        if (card.Chapter is not null || card.NarrativePosition is not null)
            writer.WriteLine($"Manuscript: {card.Chapter} {card.Act} position {card.NarrativePosition}".Trim());

        if (card.StoryTime?.Date is { } start)
        {
            var end = card.Duration is { } duration ? start + duration.Value : (DateTime?)null;
            var range = end is { } endValue
                ? $"{start:yyyy-MM-dd HH:mm}–{endValue:HH:mm}"
                : start.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            writer.WriteLine($"Story time: {range}");
        }

        if (card.LocationId is not null)
            writer.WriteLine($"Location:   {LocationName(workspace, card.LocationId.Value.Value)}");
        if (card.PovParticipantId is not null)
            writer.WriteLine($"POV:        {ParticipantName(workspace, card.PovParticipantId.Value.Value)}");

        writer.WriteLine("Participants:");
        foreach (var participant in card.ParticipantIds)
            writer.WriteLine($"  - {ParticipantName(workspace, participant.Value)}");

        if (card.Plots.Count > 0)
        {
            writer.WriteLine("Plot threads:");
            foreach (var plot in card.Plots)
                writer.WriteLine($"  - {PlotName(workspace, plot.PlotId.Value)}: {plot.Classification.ToString().ToLowerInvariant()}");
        }

        writer.WriteLine("Continuity:");
        if (card.Previous is not null)
            writer.WriteLine($"  Previous scene: {card.Previous.SceneId}, {NeighborLocation(workspace, card.Previous)}, {card.Previous.StoryDateTime:HH:mm}");
        else
            writer.WriteLine("  Previous scene: none");
        if (card.Next is not null)
            writer.WriteLine($"  Next scene:     {card.Next.SceneId}, {NeighborLocation(workspace, card.Next)}, {card.Next.StoryDateTime:HH:mm}");
        else
            writer.WriteLine("  Next scene:     none");
        if (card.ElapsedSincePrevious is { } elapsed)
            writer.WriteLine($"  Elapsed time since previous: {FormatElapsed(elapsed)}");

        if (card.Flags.Count > 0)
        {
            writer.WriteLine("Flags:");
            foreach (var flag in card.Flags)
                writer.WriteLine($"  ! {flag}");
        }

        if (card.Notes is not null)
        {
            writer.WriteLine("Summary:");
            writer.WriteLine($"  {card.Notes}");
        }
    }

    private static string NeighborLocation(NovelWorkspace workspace, SceneCardNeighbor neighbor) =>
        neighbor.LocationId is null ? "" : LocationName(workspace, neighbor.LocationId);

    private static string LocationName(NovelWorkspace workspace, string id) =>
        workspace.Locations.GetValueOrDefault(id)?.Name ?? id;

    private static string ParticipantName(NovelWorkspace workspace, string id) =>
        workspace.Participants.GetValueOrDefault(id)?.Name ?? id;

    private static string PlotName(NovelWorkspace workspace, string id) =>
        workspace.Plots.GetValueOrDefault(id)?.Description ?? id;

    private static string FormatElapsed(TimeSpan elapsed) =>
        elapsed.TotalHours >= 1
            ? $"{(int)elapsed.TotalHours}h {elapsed.Minutes}m"
            : $"{elapsed.Minutes}m";
}