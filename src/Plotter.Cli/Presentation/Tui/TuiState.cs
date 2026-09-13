using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Tui;

public sealed class TuiState
{
  public string? SelectedSceneId { get; private set; }

  public string CurrentView { get; private set; } = "timeline";

  public string? Filter { get; set; }

  public string Ordering { get; set; } = "story-time";

  public void Select(string? sceneId) => SelectedSceneId = sceneId;

  /// <summary>Keeps the selection stable when it remains in the result set; otherwise falls back deterministically.</summary>
  public void PreserveOrFallback(IReadOnlySet<string> availableSceneIds)
  {
    if (SelectedSceneId is not null && availableSceneIds.Contains(SelectedSceneId))
      return;
    SelectedSceneId = availableSceneIds.OrderBy(id => id, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
  }

  public void ChangeView(string view, IReadOnlySet<string> availableSceneIds)
  {
    CurrentView = view;
    PreserveOrFallback(availableSceneIds);
  }
}

public sealed class TuiQueryAdapter
{
  public static IReadOnlySet<string> SceneIdsForView(NovelWorkspace workspace, string view, string? filter, string ordering)
  {
    IEnumerable<string> ids = view switch
    {
      "manuscript" => TimelineQueries.GetManuscript(workspace).Rows.Select(row => row.SceneId),
      _ => TimelineQueries.GetChronological(workspace, new SceneListQuery(Order: ordering)).Scenes.Select(scene => scene.SceneId),
    };

    if (!string.IsNullOrWhiteSpace(filter))
      ids = ids.Where(id => id.Contains(filter, StringComparison.OrdinalIgnoreCase));

    return ids.ToHashSet(StringComparer.OrdinalIgnoreCase);
  }
}
