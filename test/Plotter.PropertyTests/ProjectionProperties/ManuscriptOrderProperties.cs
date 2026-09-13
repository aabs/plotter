using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class ManuscriptOrderProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ManuscriptRowsAreOrderedByNarrativePosition(NovelWorkspace workspace)
  {
    var positions = workspace.Scenes.ToDictionary(
        pair => pair.Key,
        pair => pair.Value.NarrativePosition ?? int.MaxValue,
        StringComparer.OrdinalIgnoreCase);

    var rows = TimelineQueries.GetManuscript(workspace).Rows;
    for (var index = 1; index < rows.Count; index++)
    {
      var previous = positions[rows[index - 1].SceneId];
      var current = positions[rows[index].SceneId];
      if (current < previous)
        return false;
      if (current == previous
          && string.Compare(rows[index - 1].SceneId, rows[index].SceneId, StringComparison.OrdinalIgnoreCase) > 0)
        return false;
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool FlashbackRowsPrecedePreviousDatedScene(NovelWorkspace workspace)
  {
    var rows = TimelineQueries.GetManuscript(workspace).Rows;
    DateTime? previousDate = null;
    foreach (var row in rows)
    {
      if (row.IsFlashback
          && (previousDate is null || row.StoryDateTime is not { } date || date >= previousDate))
        return false;
      if (row.StoryDateTime is { } dated)
        previousDate = dated;
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SameActRowsAreContiguous(NovelWorkspace workspace)
  {
    var acts = TimelineQueries.GetManuscript(workspace).Rows.Select(row => row.Act ?? "Unassigned").ToArray();
    var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    string? previous = null;
    foreach (var act in acts)
    {
      if (previous is not null && !act.Equals(previous, StringComparison.OrdinalIgnoreCase) && seen.Contains(act))
        return false;
      seen.Add(act);
      previous = act;
    }
    return true;
  }
}
