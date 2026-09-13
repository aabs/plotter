using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class SceneCardProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool PreviousIsImmediatelyEarlierDatedScene(NovelWorkspace workspace)
  {
    var datedIds = workspace.Scenes.Values
        .Where(scene => scene.StoryTime?.Date is not null)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .Select(scene => scene.Id.Value)
        .ToArray();
    if (datedIds.Length < 2)
      return true;

    var card = SceneDetailQueries.GetSceneDetail(workspace, datedIds[1]);
    return card is not null && card.Previous?.SceneId == datedIds[0];
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool NextIsImmediatelyLaterDatedScene(NovelWorkspace workspace)
  {
    var datedIds = workspace.Scenes.Values
        .Where(scene => scene.StoryTime?.Date is not null)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .Select(scene => scene.Id.Value)
        .ToArray();
    if (datedIds.Length < 2)
      return true;

    var card = SceneDetailQueries.GetSceneDetail(workspace, datedIds[^2]);
    return card is not null && card.Next?.SceneId == datedIds[^1];
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ElapsedEqualsDifferenceFromPrevious(NovelWorkspace workspace)
  {
    var dated = workspace.Scenes.Values
        .Where(scene => scene.StoryTime?.Date is not null)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .ToArray();
    for (var index = 1; index < dated.Length; index++)
    {
      var card = SceneDetailQueries.GetSceneDetail(workspace, dated[index].Id.Value);
      var expected = dated[index].StoryTime!.Date!.Value - dated[index - 1].StoryTime!.Date!.Value;
      if (card is null || card.ElapsedSincePrevious != expected)
        return false;
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool FirstDatedSceneHasNoPrevious(NovelWorkspace workspace)
  {
    var first = workspace.Scenes.Values
        .Where(scene => scene.StoryTime?.Date is not null)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .FirstOrDefault();
    if (first is null)
      return true;
    var card = SceneDetailQueries.GetSceneDetail(workspace, first.Id.Value);
    return card is not null && card.Previous is null;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool UndatedSceneHasNoNeighbors(NovelWorkspace workspace)
  {
    var undated = workspace.Scenes.Values.FirstOrDefault(scene => scene.StoryTime?.Date is null);
    if (undated is null)
      return true;
    var card = SceneDetailQueries.GetSceneDetail(workspace, undated.Id.Value);
    return card is not null && card.Previous is null && card.Next is null && card.ElapsedSincePrevious is null;
  }
}
