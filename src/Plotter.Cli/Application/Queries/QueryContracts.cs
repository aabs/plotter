using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public sealed record SceneListQuery(string? Order = null, string? From = null, string? To = null, string? Character = null);

public sealed record SceneRow(string SceneId, string? Title, DateTime? StoryDateTime, string? LocationId, IReadOnlyList<string> ParticipantIds, string? TemporalStatus = null);

public sealed record SceneListResult(IReadOnlyList<SceneRow> Scenes);

public sealed record ManuscriptRow(string SceneId, string? Title, string? Act, string? Chapter, DateTime? StoryDateTime, bool IsFlashback);

public sealed record ManuscriptResult(IReadOnlyList<ManuscriptRow> Rows);

public sealed record ItineraryRow(DateTime? Time, string? LocationId, string SceneId, string? Title);

public sealed record ItineraryResult(string ParticipantId, IReadOnlyList<ItineraryRow> Rows);

public sealed record LaneOccurrence(string SceneId, string? LocationId);

public sealed record LaneCell(IReadOnlyList<LaneOccurrence> Occurrences);

public sealed record LaneRow(DateTime? Time, IReadOnlyList<LaneCell> Cells);

public sealed record LaneResult(IReadOnlyList<string> ParticipantIds, IReadOnlyList<LaneRow> Rows);

public sealed record SceneDetailResult(
    SceneId SceneId,
    string? Title,
    StoryTime? StoryTime,
    SceneDuration? Duration,
    LocationId? LocationId,
    IReadOnlyList<ParticipantId> ParticipantIds);

public interface IProjectionQueryService
{
    Task<SceneListResult> GetSceneListAsync(SceneListQuery query, CancellationToken cancellationToken = default);

    Task<SceneDetailResult> GetSceneDetailAsync(SceneId sceneId, CancellationToken cancellationToken = default);
}