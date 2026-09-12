using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public sealed record SceneListQuery(string? Order = null, string? From = null, string? To = null, string? Character = null);

public sealed record SceneRow(string SceneId, string? Title, DateTime? StoryDateTime, string? LocationId, IReadOnlyList<string> ParticipantIds);

public sealed record SceneListResult(IReadOnlyList<SceneRow> Scenes);

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