using System.Globalization;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class TimelineQueries
{
  public static SceneListResult GetChronological(NovelWorkspace workspace, SceneListQuery query)
  {
    var from = ParseDate(query.From);
    var to = ParseDate(query.To);

    if (from is not null && to is not null && from > to)
      return new SceneListResult([]);

    var scenes = workspace.Scenes.Values
        .Where(scene => MatchesRange(scene, from, to))
        .OrderBy(scene => scene.StoryTime?.Date ?? DateTime.MaxValue)
        .ThenBy(scene => scene.Id.Value, StringComparer.OrdinalIgnoreCase)
        .Select(scene => new SceneRow(
            scene.Id.Value,
            scene.Title,
            scene.StoryTime?.Date,
            scene.LocationId?.Value,
            scene.ParticipantIds.Select(participant => participant.Value).ToArray(),
            DescribeTemporalStatus(scene.StoryTime)))
        .ToArray();

    return new SceneListResult(scenes);
  }

  public static ManuscriptResult GetManuscript(NovelWorkspace workspace)
  {
    var ordered = workspace.Scenes.Values
        .OrderBy(scene => scene.NarrativePosition ?? int.MaxValue)
        .ThenBy(scene => scene.Id.Value, StringComparer.OrdinalIgnoreCase)
        .ToArray();

    var rows = new List<ManuscriptRow>();
    DateTime? previousDate = null;
    foreach (var scene in ordered)
    {
      var date = scene.StoryTime?.Date;
      var isFlashback = previousDate is not null && date is not null && date < previousDate;
      rows.Add(new ManuscriptRow(scene.Id.Value, scene.Title, scene.Act, scene.Chapter, date, isFlashback));
      if (date is not null)
        previousDate = date;
    }

    return new ManuscriptResult(rows);
  }

  private static bool MatchesRange(Scene scene, DateTime? from, DateTime? to)
  {
    if (scene.StoryTime?.Date is not { } date)
      return from is null && to is null;
    var lower = from?.Date;
    var upperExclusive = to?.Date.AddDays(1);
    return (lower is null || date >= lower) && (upperExclusive is null || date < upperExclusive);
  }

  private static DateTime? ParseDate(string? value) =>
      value is null ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None);

  private static string DescribeTemporalStatus(StoryTime? story) =>
      story?.Date is null ? "undated" : story.Certainty switch
      {
        DateCertainty.Approximate => "approximate",
        DateCertainty.Unknown => "uncertain",
        _ => "dated",
      };
}
