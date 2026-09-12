using System.Globalization;
using System.Text;
using System.Text.Json;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Export;

public static class ArtifactExporters
{
    public static string ToMarkdown(SceneListResult result)
    {
        var builder = new StringBuilder("| SceneId | Title | Story time | Location |\n|---|---|---|---|\n");
        foreach (var scene in result.Scenes)
            builder.AppendLine($"| {scene.SceneId} | {scene.Title} | {scene.StoryDateTime:yyyy-MM-dd HH:mm} | {scene.LocationId} |");
        return builder.ToString();
    }

    public static string ToICal(NovelWorkspace workspace, SceneListResult result)
    {
        var builder = new StringBuilder("BEGIN:VCALENDAR\nVERSION:2.0\nPRODID:-//Plotter//EN\n");
        foreach (var scene in result.Scenes.Where(scene => scene.StoryDateTime is not null))
        {
            builder.AppendLine("BEGIN:VEVENT");
            builder.AppendLine($"UID:{scene.SceneId}@plotter");
            builder.AppendLine($"DTSTART:{scene.StoryDateTime!.Value:yyyyMMddTHHmmssZ}");
            builder.AppendLine($"SUMMARY:{Escape(scene.Title ?? scene.SceneId)}");
            builder.AppendLine("END:VEVENT");
        }
        builder.AppendLine("END:VCALENDAR");
        return builder.ToString();
    }

    public static string ToDot(NovelWorkspace workspace, string by)
    {
        var builder = new StringBuilder("digraph plotter {\n");
        if (by.Equals("locations", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var location in workspace.Locations.Values)
                builder.AppendLine($"  \"{location.Name}\";");
            foreach (var participant in workspace.Participants.Values)
            {
                var scenes = workspace.Scenes.Values
                    .Where(scene => scene.ParticipantIds.Any(id => id.Value.Equals(participant.Id.Value, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(scene => scene.StoryTime?.Date ?? DateTime.MaxValue)
                    .ToArray();
                for (var index = 1; index < scenes.Length; index++)
                {
                    if (scenes[index - 1].LocationId is { } from && scenes[index].LocationId is { } to
                        && !from.Value.Equals(to.Value, StringComparison.OrdinalIgnoreCase))
                        builder.AppendLine($"  \"{Name(workspace.Locations, from.Value)}\" -> \"{Name(workspace.Locations, to.Value)}\" [label=\"{participant.Name}\"];");
                }
            }
        }
        else
        {
            foreach (var participant in workspace.Participants.Values)
                builder.AppendLine($"  \"{participant.Name}\";");
            foreach (var scene in workspace.Scenes.Values)
            {
                var participants = scene.ParticipantIds.Select(id => Name(workspace.Participants, id.Value)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                for (var first = 0; first < participants.Length; first++)
                    for (var second = first + 1; second < participants.Length; second++)
                        builder.AppendLine($"  \"{participants[first]}\" -> \"{participants[second]}\" [label=\"{scene.Id.Value}\"];");
            }
        }
        builder.AppendLine("}");
        return builder.ToString();
    }

    public static string ToMermaid(NovelWorkspace workspace, string by)
    {
        var builder = new StringBuilder("graph LR\n");
        if (by.Equals("locations", StringComparison.OrdinalIgnoreCase))
        {
            foreach (var location in workspace.Locations.Values)
                builder.AppendLine($"  {Safe(location.Id.Value)}[\"{location.Name}\"]");
        }
        else
        {
            foreach (var participant in workspace.Participants.Values)
                builder.AppendLine($"  {Safe(participant.Id.Value)}[\"{participant.Name}\"]");
        }
        return builder.ToString();
    }

    public static string ToHtml(SceneListResult result)
    {
        var builder = new StringBuilder("<!DOCTYPE html><html><body><h1>Plotter scenes</h1><table><tr><th>SceneId</th><th>Title</th></tr>");
        foreach (var scene in result.Scenes)
            builder.AppendLine($"<tr><td>{System.Net.WebUtility.HtmlEncode(scene.SceneId)}</td><td>{System.Net.WebUtility.HtmlEncode(scene.Title ?? string.Empty)}</td></tr>");
        builder.AppendLine("</table></body></html>");
        return builder.ToString();
    }

    public static string ToSvg(SceneListResult result)
    {
        var builder = new StringBuilder("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"600\" height=\"300\">");
        var y = 20;
        foreach (var scene in result.Scenes)
        {
            builder.AppendLine($"<rect x=\"10\" y=\"{y}\" width=\"580\" height=\"14\" fill=\"#eef\" stroke=\"#333\" />");
            builder.AppendLine($"<text x=\"15\" y=\"{y + 12}\" font-size=\"12\">{System.Net.WebUtility.HtmlEncode(scene.SceneId)} {System.Net.WebUtility.HtmlEncode(scene.Title ?? string.Empty)}</text>");
            y += 18;
        }
        builder.AppendLine("</svg>");
        return builder.ToString();
    }

    public static string ToSarifJson(Application.Commands.AuditFinding[] findings) =>
        JsonSerializer.Serialize(new { version = "2.1.0", runs = new[] { new { tool = new { driver = new { name = "Plotter", version = "0.1.0" } }, results = findings } } }, BasicExporters.JsonOptions);

    private static string Name(IReadOnlyDictionary<string, Location> locations, string id) =>
        locations.GetValueOrDefault(id)?.Name ?? id;

    private static string Name(IReadOnlyDictionary<string, Participant> participants, string id) =>
        participants.GetValueOrDefault(id)?.Name ?? id;

    private static string Escape(string value) => value.Replace(";", "\\;").Replace(",", "\\,").Replace("\n", " ");

    private static string Safe(string value) => value.Replace(" ", "_");
}
