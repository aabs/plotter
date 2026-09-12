using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

/// <summary>Parameters controlling a scene-list projection.</summary>
public sealed record SceneListQuery(string? Order = null, string? From = null, string? To = null, string? Character = null);

/// <summary>A single scene row in a chronological or filtered scene list.</summary>
public sealed record SceneRow(string SceneId, string? Title, DateTime? StoryDateTime, string? LocationId, IReadOnlyList<string> ParticipantIds, string? TemporalStatus = null);

/// <summary>The result of a chronological scene-list query.</summary>
public sealed record SceneListResult(IReadOnlyList<SceneRow> Scenes);

/// <summary>A scene row in a manuscript-order projection.</summary>
public sealed record ManuscriptRow(string SceneId, string? Title, string? Act, string? Chapter, DateTime? StoryDateTime, bool IsFlashback);

/// <summary>The result of a manuscript-order projection.</summary>
public sealed record ManuscriptResult(IReadOnlyList<ManuscriptRow> Rows);

/// <summary>A single entry in a participant's chronological itinerary.</summary>
public sealed record ItineraryRow(DateTime? Time, string? LocationId, string SceneId, string? Title);

/// <summary>A participant's chronological itinerary.</summary>
public sealed record ItineraryResult(string ParticipantId, IReadOnlyList<ItineraryRow> Rows);

/// <summary>A single scene occurrence in a character-lane cell.</summary>
public sealed record LaneOccurrence(string SceneId, string? LocationId);

/// <summary>A character-lane cell; an empty occurrence list means the participant is absent.</summary>
public sealed record LaneCell(IReadOnlyList<LaneOccurrence> Occurrences);

/// <summary>A time row in a character-lane matrix.</summary>
public sealed record LaneRow(DateTime? Time, IReadOnlyList<LaneCell> Cells);

/// <summary>A character-lane matrix result with one cell per participant per time row.</summary>
public sealed record LaneResult(IReadOnlyList<string> ParticipantIds, IReadOnlyList<LaneRow> Rows);

/// <summary>An occupied time range for a scene at a location.</summary>
public sealed record LocationTimelineRow(DateTime Start, DateTime? End, string SceneId, string? Title);

/// <summary>The chronological timeline of scenes occupying a location.</summary>
public sealed record LocationTimelineResult(string LocationId, IReadOnlyList<LocationTimelineRow> Rows);

/// <summary>Density metadata for a location: scene count and first/last use dates.</summary>
public sealed record LocationDensityRow(string LocationId, int SceneCount, DateTime? FirstUse, DateTime? LastUse);

/// <summary>The location density summary for a workspace.</summary>
public sealed record LocationDensityResult(IReadOnlyList<LocationDensityRow> Rows);

/// <summary>The active location of a participant at a point in time, including any conflicts.</summary>
public sealed record WhereResult(string ParticipantId, string? LocationId, IReadOnlyList<string> ConflictLocationIds);

/// <summary>A scene-to-plot relationship cell in the plot-thread matrix.</summary>
public sealed record ThreadMatrixCell(PlotThreadClassification Classification, string? Annotation);

/// <summary>A scene row in the plot-thread matrix keyed by plot ID.</summary>
public sealed record ThreadMatrixRow(string SceneId, IReadOnlyDictionary<string, ThreadMatrixCell> Cells);

/// <summary>The scene-by-plot-thread matrix result.</summary>
public sealed record ThreadMatrixResult(IReadOnlyList<string> PlotIds, IReadOnlyList<ThreadMatrixRow> Rows);

/// <summary>A single scene entry in a per-thread timeline.</summary>
public sealed record ThreadTimelineRow(string SceneId, string? Title, string? Annotation);

/// <summary>The per-plot-thread timeline result.</summary>
public sealed record ThreadTimelineResult(string PlotId, IReadOnlyList<ThreadTimelineRow> Rows);

/// <summary>The previous or next dated scene relative to a scene card.</summary>
public sealed record SceneCardNeighbor(string SceneId, string? Title, string? LocationId, DateTime? StoryDateTime);

/// <summary>The full structured detail card for a scene, including continuity context.</summary>
public sealed record SceneCardDetail(
    SceneId SceneId,
    string? Title,
    int? NarrativePosition,
    string? Act,
    string? Chapter,
    StoryTime? StoryTime,
    SceneDuration? Duration,
    LocationId? LocationId,
    IReadOnlyList<ParticipantId> ParticipantIds,
    IReadOnlyList<PlotRelationship> Plots,
    ParticipantId? PovParticipantId,
    string? Status,
    string? Notes,
    SceneCardNeighbor? Previous,
    SceneCardNeighbor? Next,
    TimeSpan? ElapsedSincePrevious,
    IReadOnlyList<string> Flags);

/// <summary>A dated scene projected as a calendar event.</summary>
public sealed record CalendarEvent(string SceneId, string? Title, DateTime? StoryDateTime, string? LocationId);

/// <summary>The calendar events within a requested period.</summary>
public sealed record CalendarResult(IReadOnlyList<CalendarEvent> Events);

/// <summary>A single interaction in a participant's history.</summary>
public sealed record InteractionHistoryRow(string SceneId, string? Type, string? Description, IReadOnlyList<string> ParticipantIds);

/// <summary>A participant's chronological interaction history.</summary>
public sealed record InteractionHistoryResult(string ParticipantId, IReadOnlyList<InteractionHistoryRow> Rows);

/// <summary>A step in a participant's travel projection.</summary>
public sealed record TravelStep(DateTime? Time, string? LocationId, TimeSpan? AvailableUntilNext, TimeSpan? ModeledRoute);

/// <summary>A participant's travel projection across consecutive dated scenes.</summary>
public sealed record TravelResult(string ParticipantId, IReadOnlyList<TravelStep> Steps);

/// <summary>A gap between two consecutive dated scenes with its classification.</summary>
public sealed record GapRow(string FromSceneId, string ToSceneId, string FromContext, string ToContext, TimeSpan? Gap, GapClass GapClass);

/// <summary>A scene detail projection.</summary>
public sealed record SceneDetailResult(
    SceneId SceneId,
    string? Title,
    StoryTime? StoryTime,
    SceneDuration? Duration,
    LocationId? LocationId,
    IReadOnlyList<ParticipantId> ParticipantIds);

/// <summary>Query service contract for projecting scene lists and details.</summary>
public interface IProjectionQueryService
{
    Task<SceneListResult> GetSceneListAsync(SceneListQuery query, CancellationToken cancellationToken = default);

    Task<SceneDetailResult> GetSceneDetailAsync(SceneId sceneId, CancellationToken cancellationToken = default);
}
