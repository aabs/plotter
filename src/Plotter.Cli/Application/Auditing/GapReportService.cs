using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Auditing;

public static class GapReportService
{
  public static IReadOnlyList<GapRow> GetGaps(NovelWorkspace workspace)
  {
    var dated = workspace.Scenes.Values
        .Where(scene => scene.StoryTime?.Date is not null)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .ToArray();

    var rows = new List<GapRow>();
    for (var index = 1; index < dated.Length; index++)
    {
      var previous = dated[index - 1];
      var current = dated[index];
      var previousStart = previous.StoryTime!.Date!.Value;

      if (previous.Duration is null)
      {
        rows.Add(new GapRow(
            previous.Id.Value,
            current.Id.Value,
            Describe(previous),
            Describe(current),
            null,
            GapClass.Unknown));
        continue;
      }

      var previousEnd = previousStart + previous.Duration.Value.Value;
      var gap = current.StoryTime!.Date!.Value - previousEnd;
      rows.Add(new GapRow(
          previous.Id.Value,
          current.Id.Value,
          Describe(previous),
          Describe(current),
          gap,
          Classify(gap)));
    }

    return rows;
  }

  public static GapClass Classify(TimeSpan gap) =>
      gap < TimeSpan.FromHours(6) ? GapClass.Normal
      : gap < TimeSpan.FromHours(24) ? GapClass.Long
      : GapClass.Overnight;

  private static string Describe(Scene scene) =>
      $"{scene.LocationId?.Value ?? "?"}, {scene.StoryTime!.Date!.Value:HH:mm}";
}
