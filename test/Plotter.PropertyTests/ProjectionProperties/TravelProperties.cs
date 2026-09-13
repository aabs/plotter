using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Travel;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class TravelProperties
{
  private sealed class StubTravelModel : ITravelModelAdapter
  {
    public TimeSpan? Estimate(string fromLocationId, string toLocationId) => TimeSpan.FromMinutes(10);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool AvailableTimeEqualsDifferenceFromPreviousEnd(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var result = TravelQueries.GetTravel(workspace, participant, new NullTravelModel());
    for (var index = 0; index < result.Steps.Count - 1; index++)
    {
      var current = result.Steps[index];
      var next = result.Steps[index + 1];
      if (current.Time is not { } currentTime || next.Time is not { } nextTime)
        return false;
      var scene = workspace.Scenes.Values.First(candidate =>
          candidate.StoryTime?.Date == currentTime && candidate.ParticipantIds.Any(p => p.Value.Equals(participant, StringComparison.OrdinalIgnoreCase)));
      var expected = nextTime - (currentTime + (scene.Duration?.Value ?? TimeSpan.Zero));
      if (current.AvailableUntilNext != expected)
        return false;
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool NoModelNeverInventsRoutes(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var result = TravelQueries.GetTravel(workspace, participant, new NullTravelModel());
    return result.Steps.All(step => step.ModeledRoute is null);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ModeledRoutesAppearWhenModelProvided(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var result = TravelQueries.GetTravel(workspace, participant, new StubTravelModel());
    foreach (var step in result.Steps.Where(step => step.LocationId is not null))
      if (step.AvailableUntilNext is not null && step.ModeledRoute != TimeSpan.FromMinutes(10))
        return false;
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool TravelStepsAreOrderedByTime(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var steps = TravelQueries.GetTravel(workspace, participant, new NullTravelModel()).Steps;
    for (var index = 1; index < steps.Count; index++)
      if (steps[index].Time < steps[index - 1].Time)
        return false;
    return true;
  }
}
