using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class SceneCardCommandProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool CardRendersSceneIdAndTitle(NovelWorkspace workspace)
  {
    var scene = workspace.Scenes.Values.FirstOrDefault();
    if (scene is null)
      return true;
    var card = SceneDetailQueries.GetSceneDetail(workspace, scene.Id.Value);
    if (card is null)
      return false;
    var writer = new StringWriter();
    SceneCardRenderer.RenderCard(workspace, card, writer);
    var output = writer.ToString();
    return output.Contains(scene.Id.Value, StringComparison.Ordinal)
        && (scene.Title is null || output.Contains(scene.Title, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool UnknownElapsedTimeRendersWithoutValue(NovelWorkspace workspace)
  {
    var undated = workspace.Scenes.Values.FirstOrDefault(scene => scene.StoryTime?.Date is null);
    if (undated is null)
      return true;
    var card = SceneDetailQueries.GetSceneDetail(workspace, undated.Id.Value);
    if (card is null)
      return false;
    var writer = new StringWriter();
    SceneCardRenderer.RenderCard(workspace, card, writer);
    var output = writer.ToString();
    return !output.Contains("Elapsed time since previous: 0", StringComparison.Ordinal)
        && (card.ElapsedSincePrevious is null || output.Contains("Elapsed time since previous:", StringComparison.Ordinal));
  }
}
