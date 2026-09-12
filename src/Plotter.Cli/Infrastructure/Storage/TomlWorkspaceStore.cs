using System.Globalization;
using System.Text;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Infrastructure.Storage;

internal sealed class TomlWorkspaceStore
{
    public static async Task<NovelWorkspace> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        var workspace = new NovelWorkspace();
        if (!File.Exists(path)) return workspace;
        var section = string.Empty;
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        await foreach (var line in File.ReadLinesAsync(path, cancellationToken))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#')) continue;
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                Flush(section, values, workspace);
                values.Clear();
                section = trimmed[1..^1];
                continue;
            }
            var separator = trimmed.IndexOf('=');
            if (separator > 0)
                values[trimmed[..separator].Trim()] = Unquote(trimmed[(separator + 1)..].Trim());
        }
        Flush(section, values, workspace);
        return workspace;
    }

    public static async Task SaveAsync(string path, NovelWorkspace workspace, CancellationToken cancellationToken = default)
    {
        var builder = new StringBuilder("# Plotter novel workspace\nformat_version = \"1\"\n\n");
        foreach (var participant in workspace.Participants.Values)
            builder.AppendLine($"[participants.{participant.Id}]\nname = \"{Escape(participant.Name)}\"\n");
        foreach (var location in workspace.Locations.Values)
            builder.AppendLine($"[locations.{location.Id}]\nname = \"{Escape(location.Name)}\"\n");
        foreach (var scene in workspace.Scenes.Values.OrderBy(s => s.StoryDateTime ?? DateTime.MaxValue).ThenBy(s => s.Id, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"[scenes.{scene.Id}]");
            if (scene.Title is not null) builder.AppendLine($"title = \"{Escape(scene.Title)}\"");
            if (scene.StoryDateTime is not null) builder.AppendLine($"story_date_time = \"{scene.StoryDateTime.Value:O}\"");
            if (scene.Duration is not null) builder.AppendLine($"duration_minutes = {scene.Duration.Value.TotalMinutes.ToString(CultureInfo.InvariantCulture)}");
            if (scene.LocationId is not null) builder.AppendLine($"location_id = \"{Escape(scene.LocationId)}\"");
            if (scene.ParticipantIds.Count > 0) builder.AppendLine($"participant_ids = [{string.Join(", ", scene.ParticipantIds.Select(p => $"\"{Escape(p)}\""))}]");
            if (scene.Notes is not null) builder.AppendLine($"notes = \"{Escape(scene.Notes)}\"");
            builder.AppendLine();
        }
        var temporary = path + ".tmp";
        await File.WriteAllTextAsync(temporary, builder.ToString(), cancellationToken);
        File.Move(temporary, path, true);
    }

    private static void Flush(string section, Dictionary<string, string> values, NovelWorkspace workspace)
    {
        var parts = section.Split('.', 2);
        if (parts.Length != 2) return;
        var id = parts[1];
        if (parts[0].Equals("participants", StringComparison.OrdinalIgnoreCase))
            workspace.Participants[id] = new Participant(id, values.GetValueOrDefault("name", id));
        else if (parts[0].Equals("locations", StringComparison.OrdinalIgnoreCase))
            workspace.Locations[id] = new Location(id, values.GetValueOrDefault("name", id));
        else if (parts[0].Equals("scenes", StringComparison.OrdinalIgnoreCase))
        {
            DateTime? date = values.TryGetValue("story_date_time", out var dateValue) && DateTime.TryParse(dateValue, null, DateTimeStyles.RoundtripKind, out var parsed) ? parsed : null;
            TimeSpan? duration = values.TryGetValue("duration_minutes", out var durationValue) && double.TryParse(durationValue, CultureInfo.InvariantCulture, out var minutes) ? TimeSpan.FromMinutes(minutes) : null;
            var participants = values.TryGetValue("participant_ids", out var participantValue) ? participantValue.Trim('[', ']').Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Unquote).ToArray() : [];
            workspace.Scenes[id] = new Scene(id, values.GetValueOrDefault("title"), date, duration, values.GetValueOrDefault("location_id"), participants, values.GetValueOrDefault("notes"));
        }
    }

    private static string Unquote(string value) => value.Trim().Trim('"');
    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
