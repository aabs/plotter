using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class LocationQueries
{
  public static LocationTimelineResult GetLocationTimeline(NovelWorkspace workspace, string locationId)
  {
    var rows = workspace.Scenes.Values
        .Where(scene => scene.LocationId?.Value.Equals(locationId, StringComparison.OrdinalIgnoreCase) == true)
        .Where(scene => scene.StoryTime?.Date is not null)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .Select(scene =>
        {
          var start = scene.StoryTime!.Date!.Value;
          DateTime? end = scene.Duration is { } duration ? start + duration.Value : null;
          return new LocationTimelineRow(start, end, scene.Id.Value, scene.Title);
        })
        .ToArray();
    return new LocationTimelineResult(locationId, rows);
  }

  public static LocationDensityResult GetLocationDensity(NovelWorkspace workspace)
  {
    var rows = workspace.Locations.Values
        .Select(location =>
        {
          var scenes = workspace.Scenes.Values
                  .Where(scene => scene.LocationId?.Value.Equals(location.Id.Value, StringComparison.OrdinalIgnoreCase) == true)
                  .ToArray();
          var dates = scenes
                  .Select(scene => scene.StoryTime?.Date)
                  .Where(date => date is not null)
                  .Select(date => date!.Value)
                  .ToArray();
          return new LocationDensityRow(
                  location.Id.Value,
                  scenes.Length,
                  dates.Length == 0 ? null : dates.Min(),
                  dates.Length == 0 ? null : dates.Max());
        })
        .OrderByDescending(row => row.SceneCount)
        .ToArray();
    return new LocationDensityResult(rows);
  }

  public static WhereResult GetActiveLocation(NovelWorkspace workspace, string participantId, DateTime at)
  {
    var locations = new List<string>();
    foreach (var scene in workspace.Scenes.Values)
    {
      if (!scene.ParticipantIds.Any(participant => participant.Value.Equals(participantId, StringComparison.OrdinalIgnoreCase)))
        continue;
      if (scene.StoryTime?.Date is not { } start || scene.LocationId is not { } location)
        continue;

      if (scene.Duration is { } duration)
      {
        var end = start + duration.Value;
        if (at >= start && at < end)
          locations.Add(location.Value);
        continue;
      }

      var nextSceneStart = workspace.Scenes.Values
          .Where(candidate => candidate.Id.Value != scene.Id.Value
              && candidate.ParticipantIds.Any(participant => participant.Value.Equals(participantId, StringComparison.OrdinalIgnoreCase))
              && candidate.StoryTime?.Date is { } candidateDate && candidateDate > start)
          .OrderBy(candidate => candidate.StoryTime!.Date!.Value)
          .Select(candidate => candidate.StoryTime!.Date!.Value)
          .FirstOrDefault();

      var inferredEnd = nextSceneStart == default ? null : (DateTime?)nextSceneStart;
      if (at >= start && (inferredEnd is null || at < inferredEnd))
        locations.Add(location.Value);
    }

    var distinct = locations.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    return new WhereResult(participantId, distinct.Length == 1 ? distinct[0] : null, distinct.Length > 1 ? distinct : []);
  }
}
