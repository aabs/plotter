namespace Plotter.Cli.Domain;

public static class Validation
{
    public static string RequiredId(string value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{label} must be non-empty.", nameof(value));
        return value.Trim();
    }

    public static void EnsureUnique<T>(IReadOnlyDictionary<string, T> values, string id, string label)
    {
        if (values.ContainsKey(id))
            throw new InvalidOperationException($"{label} '{id}' already exists.");
    }

    public static void EnsureSceneReferences(NovelWorkspace workspace, Scene scene)
    {
        if (scene.Duration is { } duration && duration < TimeSpan.Zero)
            throw new ArgumentException("Scene duration cannot be negative.");
        if (scene.LocationId is not null && !workspace.Locations.ContainsKey(scene.LocationId))
            throw new InvalidOperationException($"Location '{scene.LocationId}' does not exist.");
        foreach (var participantId in scene.ParticipantIds)
            if (!workspace.Participants.ContainsKey(participantId))
                throw new InvalidOperationException($"Participant '{participantId}' does not exist.");
    }
}
