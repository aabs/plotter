using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Tui;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class TuiProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SelectionPreservedWhenStillPresent(NovelWorkspace workspace)
  {
    var state = new TuiState();
    var timelineIds = TuiQueryAdapter.SceneIdsForView(workspace, "timeline", null, "story-time");
    var manuscriptIds = TuiQueryAdapter.SceneIdsForView(workspace, "manuscript", null, "story-time");
    var shared = timelineIds.Intersect(manuscriptIds, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
    if (shared is null)
      return true;
    state.Select(shared);
    state.ChangeView("manuscript", manuscriptIds);
    return state.SelectedSceneId == shared;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool FilteredSelectionFallsBackDeterministically(NovelWorkspace workspace)
  {
    var state = new TuiState();
    var all = TuiQueryAdapter.SceneIdsForView(workspace, "timeline", null, "story-time");
    if (all.Count == 0)
      return true;
    var selected = all.OrderBy(id => id, StringComparer.OrdinalIgnoreCase).First();
    state.Select(selected);
    state.Filter = "__no_such_scene_filter__";
    var filtered = TuiQueryAdapter.SceneIdsForView(workspace, "timeline", state.Filter, "story-time");
    state.PreserveOrFallback(filtered);
    return filtered.Count == 0
        ? state.SelectedSceneId is null
        : state.SelectedSceneId == filtered.OrderBy(id => id, StringComparer.OrdinalIgnoreCase).First();
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool AdapterMatchesChronologicalQuery(NovelWorkspace workspace)
  {
    var ids = TuiQueryAdapter.SceneIdsForView(workspace, "timeline", null, "story-time");
    var expected = TimelineQueries.GetChronological(workspace, new SceneListQuery())
        .Scenes.Select(scene => scene.SceneId)
        .ToHashSet(StringComparer.OrdinalIgnoreCase);
    return ids.SetEquals(expected);
  }
}
