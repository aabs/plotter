using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Export;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class ArtifactExporterProperties
{
  private static SceneListResult Chronological(NovelWorkspace workspace) =>
      TimelineQueries.GetChronological(workspace, new SceneListQuery());

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool MarkdownContainsSceneIds(NovelWorkspace workspace)
  {
    var result = Chronological(workspace);
    var output = ArtifactExporters.ToMarkdown(result);
    return result.Scenes.All(scene => output.Contains(scene.SceneId, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ICalContainsDatedSceneIds(NovelWorkspace workspace)
  {
    var result = Chronological(workspace);
    var output = ArtifactExporters.ToICal(workspace, result);
    var dated = result.Scenes.Where(scene => scene.StoryDateTime is not null).ToArray();
    return dated.All(scene => output.Contains(scene.SceneId, StringComparison.Ordinal))
        && output.StartsWith("BEGIN:VCALENDAR", StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool DotContainsParticipantNames(NovelWorkspace workspace)
  {
    var output = ArtifactExporters.ToDot(workspace, "participants");
    return workspace.Participants.Values.All(participant => output.Contains(participant.Name, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool DotByLocationsContainsLocationNames(NovelWorkspace workspace)
  {
    var output = ArtifactExporters.ToDot(workspace, "locations");
    return workspace.Locations.Values.All(location => output.Contains(location.Name, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool MermaidStartsWithGraph(NovelWorkspace workspace)
  {
    var output = ArtifactExporters.ToMermaid(workspace, "participants");
    return output.StartsWith("graph", StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool HtmlContainsSceneIds(NovelWorkspace workspace)
  {
    var result = Chronological(workspace);
    var output = ArtifactExporters.ToHtml(result);
    return result.Scenes.All(scene => output.Contains(scene.SceneId, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SvgContainsSceneIds(NovelWorkspace workspace)
  {
    var result = Chronological(workspace);
    var output = ArtifactExporters.ToSvg(result);
    return result.Scenes.All(scene => output.Contains(scene.SceneId, StringComparison.Ordinal));
  }
}
