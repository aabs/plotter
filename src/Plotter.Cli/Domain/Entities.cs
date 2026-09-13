namespace Plotter.Cli.Domain;

public sealed record Participant(ParticipantId Id, string Name, IReadOnlyList<GroupId>? GroupIds = null)
{
  public IReadOnlyList<GroupId> GroupIds { get; init; } = GroupIds ?? [];
}

public sealed record ParticipantGroup(GroupId Id, string Name, IReadOnlyList<ParticipantId>? ParticipantIds = null)
{
  public IReadOnlyList<ParticipantId> ParticipantIds { get; init; } = ParticipantIds ?? [];
}

public sealed record Location(LocationId Id, string Name);

public sealed record Plot(PlotId Id, string? Description = null, StoryTime? StartTime = null, StoryTime? EndTime = null);

public sealed record PlotRelationship(PlotId PlotId, PlotThreadClassification Classification, string? Annotation = null);

public sealed record Interaction(string? Type, string? Description, IReadOnlyList<ParticipantId>? ParticipantIds = null)
{
  public IReadOnlyList<ParticipantId> ParticipantIds { get; init; } = ParticipantIds ?? [];
}

public sealed record Scene(
    SceneId Id,
    string? Title = null,
    StoryTime? StoryTime = null,
    SceneDuration? Duration = null,
    LocationId? LocationId = null,
    IReadOnlyList<ParticipantId>? ParticipantIds = null,
    IReadOnlyList<PlotRelationship>? Plots = null,
    ParticipantId? PovParticipantId = null,
    string? Status = null,
    string? Notes = null,
    IReadOnlyList<ContinuityAnnotation>? ContinuityAnnotations = null,
    IReadOnlyList<Interaction>? Interactions = null,
    int? NarrativePosition = null,
    string? Act = null,
    string? Chapter = null)
{
  public IReadOnlyList<ParticipantId> ParticipantIds { get; init; } = ParticipantIds ?? [];
  public IReadOnlyList<PlotRelationship> Plots { get; init; } = Plots ?? [];
  public IReadOnlyList<ContinuityAnnotation> ContinuityAnnotations { get; init; } = ContinuityAnnotations ?? [];
  public IReadOnlyList<Interaction> Interactions { get; init; } = Interactions ?? [];
}

public sealed class NovelWorkspace
{
  public string? NovelTitle { get; set; }

  public string FormatVersion { get; set; } = "1";

  public Dictionary<string, Participant> Participants { get; } = new(StringComparer.OrdinalIgnoreCase);

  public Dictionary<string, Location> Locations { get; } = new(StringComparer.OrdinalIgnoreCase);

  public Dictionary<string, Plot> Plots { get; } = new(StringComparer.OrdinalIgnoreCase);

  public Dictionary<string, ParticipantGroup> ParticipantGroups { get; } = new(StringComparer.OrdinalIgnoreCase);

  public Dictionary<string, Scene> Scenes { get; } = new(StringComparer.OrdinalIgnoreCase);
}
