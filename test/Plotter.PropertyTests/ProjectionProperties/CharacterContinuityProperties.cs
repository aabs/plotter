using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class CharacterContinuityProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ItineraryRowsAreOrdered(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var rows = CharacterContinuityQueries.GetItinerary(workspace, participant).Rows;
    for (var index = 1; index < rows.Count; index++)
    {
      var previous = rows[index - 1].Time ?? DateTime.MaxValue;
      var current = rows[index].Time ?? DateTime.MaxValue;
      if (current < previous)
        return false;
      if (current == previous
          && string.Compare(rows[index - 1].SceneId, rows[index].SceneId, StringComparison.OrdinalIgnoreCase) > 0)
        return false;
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ItineraryContainsAllParticipantScenes(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var expected = workspace.Scenes.Values.Count(scene =>
        scene.ParticipantIds.Any(id => id.Value.Equals(participant, StringComparison.OrdinalIgnoreCase)));
    var actual = CharacterContinuityQueries.GetItinerary(workspace, participant).Rows.Count;
    return expected == actual;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool LaneRowsContainAllOccurrences(NovelWorkspace workspace)
  {
    var participants = workspace.Participants.Keys.Take(2).ToArray();
    if (participants.Length == 0)
      return true;
    var result = CharacterContinuityQueries.GetLanes(workspace, participants);
    var expected = workspace.Scenes.Values
        .SelectMany(scene => scene.ParticipantIds
            .Where(participant => participants.Contains(participant.Value, StringComparer.OrdinalIgnoreCase))
            .Select(_ => scene.Id.Value))
        .Count();
    var actual = result.Rows.Sum(row => row.Cells.Sum(cell => cell.Occurrences.Count));
    return expected == actual;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool EmptyCellsMatchAbsence(NovelWorkspace workspace)
  {
    var participants = workspace.Participants.Keys.Take(2).ToArray();
    if (participants.Length == 0)
      return true;
    var result = CharacterContinuityQueries.GetLanes(workspace, participants);
    foreach (var row in result.Rows)
    {
      for (var index = 0; index < participants.Length; index++)
      {
        var hasScene = workspace.Scenes.Values.Any(scene =>
            scene.StoryTime?.Date == row.Time
            && scene.ParticipantIds.Any(participant => participant.Value.Equals(participants[index], StringComparison.OrdinalIgnoreCase)));
        var cellEmpty = row.Cells[index].Occurrences.Count == 0;
        if (hasScene == cellEmpty)
          return false;
      }
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool RepeatedSameTimeOccurrencesArePreserved(NovelWorkspace workspace)
  {
    var participants = workspace.Participants.Keys.Take(2).ToArray();
    if (participants.Length == 0)
      return true;
    var result = CharacterContinuityQueries.GetLanes(workspace, participants);
    foreach (var row in result.Rows)
      foreach (var cell in row.Cells)
      {
        var distinct = cell.Occurrences.Select(occurrence => occurrence.SceneId).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        if (distinct != cell.Occurrences.Count)
          return false;
      }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool LocationFilterOnlyIncludesMatchingScenes(NovelWorkspace workspace)
  {
    var participants = workspace.Participants.Keys.Take(2).ToArray();
    var location = workspace.Locations.Keys.FirstOrDefault();
    if (participants.Length == 0 || location is null)
      return true;
    var result = CharacterContinuityQueries.GetLanes(workspace, participants, locationId: location);
    foreach (var row in result.Rows)
      foreach (var cell in row.Cells)
        foreach (var occurrence in cell.Occurrences)
          if (occurrence.LocationId is null || !occurrence.LocationId.Equals(location, StringComparison.OrdinalIgnoreCase))
            return false;
    return true;
  }
}
