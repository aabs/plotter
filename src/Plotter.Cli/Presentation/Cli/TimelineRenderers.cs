using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class TimelineRenderers
{
    public static void RenderChronological(NovelWorkspace workspace, SceneListResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        foreach (var group in result.Scenes.GroupBy(scene => scene.StoryDateTime?.Date))
        {
            if (group.Key is { } date)
                writer.WriteLine(date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            else
                writer.WriteLine("undated");

            foreach (var scene in group)
            {
                var time = scene.StoryDateTime?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? "??:??";
                var location = scene.LocationId is null ? "" : workspace.Locations.GetValueOrDefault(scene.LocationId)?.Name ?? scene.LocationId;
                var participants = string.Join(", ", scene.ParticipantIds.Select(id => workspace.Participants.GetValueOrDefault(id)?.Name ?? id));
                writer.WriteLine($"{time}  {scene.SceneId,-8} {scene.Title}");
                writer.WriteLine($"       {location} · {participants}");
                if (scene.TemporalStatus is not null and not "dated")
                    writer.WriteLine($"       [{scene.TemporalStatus}]");
            }

            writer.WriteLine();
        }
    }

    public static void RenderManuscript(ManuscriptResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        foreach (var group in result.Rows.GroupBy(row => row.Act ?? "Unassigned"))
        {
            writer.WriteLine(group.Key);
            foreach (var row in group)
            {
                var date = row.StoryDateTime?.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) ?? "unknown time";
                var flashback = row.IsFlashback ? "  [FLASHBACK]" : string.Empty;
                writer.WriteLine($"  {row.Chapter ?? "Ch ??"}  {row.SceneId,-8} {row.Title,-40} {date}{flashback}");
            }

            writer.WriteLine();
        }
    }
}
