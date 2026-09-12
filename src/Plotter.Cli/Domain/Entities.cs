namespace Plotter.Cli.Domain;

public sealed record Participant(string Id, string Name);
public sealed record Location(string Id, string Name);
public sealed record Scene(
    string Id,
    string? Title = null,
    DateTime? StoryDateTime = null,
    TimeSpan? Duration = null,
    string? LocationId = null,
    IReadOnlyList<string>? ParticipantIds = null,
    string? Notes = null)
{
    public IReadOnlyList<string> ParticipantIds { get; init; } = ParticipantIds ?? [];
}

public sealed class NovelWorkspace
{
    public Dictionary<string, Participant> Participants { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Location> Locations { get; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, Scene> Scenes { get; } = new(StringComparer.OrdinalIgnoreCase);
}
