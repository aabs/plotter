using System.Globalization;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class TextRenderers
{
    public static void RenderScene(NovelWorkspace workspace, Scene scene, bool compact = false)
    {
        var date = scene.StoryTime?.Date?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? "unknown time";
        var location = scene.LocationId is null ? "" : workspace.Locations.GetValueOrDefault(scene.LocationId.Value.Value)?.Name ?? scene.LocationId.Value.Value;
        var participants = string.Join(", ", scene.ParticipantIds.Select(id => workspace.Participants.GetValueOrDefault(id.Value)?.Name ?? id.Value));
        if (compact)
        {
            Console.WriteLine($"{date,16}  {scene.Id.Value,-8} {scene.Title}  {location} · {participants}");
            return;
        }
        Console.WriteLine($"{scene.Id.Value} · {scene.Title}");
        if (scene.NarrativePosition is { } position)
            Console.WriteLine($"Manuscript: position {position}");
        if (scene.Act is not null || scene.Chapter is not null)
            Console.WriteLine($"Manuscript: {scene.Act} {scene.Chapter}".TrimEnd());
        Console.WriteLine($"Story time: {date}");
        if (scene.Duration is { } duration)
            Console.WriteLine($"Duration:   {duration.Value.TotalMinutes:0}m");
        Console.WriteLine($"Location:   {location}");
        Console.WriteLine($"Participants: {participants}");
        if (scene.PovParticipantId is { } pov)
            Console.WriteLine($"POV:        {workspace.Participants.GetValueOrDefault(pov.Value)?.Name ?? pov.Value}");
        if (scene.Status is not null)
            Console.WriteLine($"Status:     {scene.Status}");
        if (scene.Notes is not null)
            Console.WriteLine($"Notes:      {scene.Notes}");
        Console.WriteLine();
    }

    public static void RenderTimeline(NovelWorkspace workspace)
    {
        foreach (var scene in workspace.Scenes.Values.OrderBy(s => s.StoryTime?.Date ?? DateTime.MaxValue).ThenBy(s => s.Id.Value, StringComparer.OrdinalIgnoreCase))
            RenderScene(workspace, scene, compact: true);
    }

    public static void RenderValidation(ValidationResult result)
    {
        foreach (var diagnostic in result.Diagnostics)
        {
            var severity = diagnostic.Severity switch
            {
                FindingSeverity.Error => "ERROR",
                FindingSeverity.Warning => "WARN",
                _ => "INFO",
            };
            Console.WriteLine($"{severity}  {diagnostic.Code}  {diagnostic.Message}");
        }
    }
}
