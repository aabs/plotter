using System.Globalization;
using System.Text.Json;
using Plotter.Cli.Application.Queries;

namespace Plotter.Cli.Presentation.Export;

public static class BasicExporters
{
    public static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static string ToText(SceneListResult result) =>
        string.Join(Environment.NewLine, result.Scenes.Select(scene => $"{scene.SceneId}\t{scene.Title}"));

    public static string ToJson(SceneListResult result) =>
        JsonSerializer.Serialize(result.Scenes, JsonOptions);

    public static string ToCsv(SceneListResult result)
    {
        var header = "SceneId,Title,StoryDateTime,LocationId";
        var rows = result.Scenes.Select(scene =>
            $"{EscapeCsv(scene.SceneId)},{EscapeCsv(scene.Title ?? string.Empty)},{scene.StoryDateTime?.ToString("O", CultureInfo.InvariantCulture) ?? string.Empty},{EscapeCsv(scene.LocationId ?? string.Empty)}");
        return string.Join(Environment.NewLine, new[] { header }.Concat(rows));
    }

    private static string EscapeCsv(string value) =>
        value.Contains(',') || value.Contains('"') ? $"\"{value.Replace("\"", "\"\"")}\"" : value;
}
