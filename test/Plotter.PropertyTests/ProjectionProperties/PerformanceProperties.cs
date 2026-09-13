using System.Diagnostics;
using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class PerformanceProperties
{
  private static NovelWorkspace LargeWorkspace(int sceneCount)
  {
    var workspace = new NovelWorkspace();
    var epoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    for (var index = 0; index < sceneCount; index++)
      workspace.Scenes[$"S{index:D4}"] = new Scene(
          new SceneId($"S{index:D4}"),
          $"Scene {index}",
          new StoryTime(epoch.AddMinutes(index)));
    return workspace;
  }

  [Property(MaxTest = 3)]
  public bool ChronologicalQueryOnThousandScenesIsFast()
  {
    var workspace = LargeWorkspace(1000);
    var stopwatch = Stopwatch.StartNew();
    var result = TimelineQueries.GetChronological(workspace, new SceneListQuery());
    stopwatch.Stop();
    return result.Scenes.Count == 1000 && stopwatch.Elapsed < TimeSpan.FromSeconds(2);
  }

  [Property(MaxTest = 5)]
  public bool ChronologicalQueryReturnsAllScenes(NonNegativeInt count)
  {
    var sceneCount = count.Get % 200;
    var workspace = LargeWorkspace(sceneCount);
    var result = TimelineQueries.GetChronological(workspace, new SceneListQuery());
    return result.Scenes.Count == sceneCount;
  }
}
