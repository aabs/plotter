using System.Text.Json;
using Plotter.Cli.Application.Queries;

namespace Plotter.Cli.Application.Projections;

public sealed class SceneListProjection : IResultProjection<SceneListResult>
{
  public string ToText(SceneListResult result) =>
      string.Join(Environment.NewLine, result.Scenes.Select(scene => $"{scene.SceneId}\t{scene.Title}"));

  public string ToJson(SceneListResult result) =>
      JsonSerializer.Serialize(result.Scenes.Select(scene => new { scene.SceneId, scene.Title }));

  public string ToCsv(SceneListResult result) =>
      string.Join(Environment.NewLine, result.Scenes.Select(scene => $"{scene.SceneId},{scene.Title}"));
}
