using System.Text.Json;
using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Presentation.Export;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class FormatEquivalenceProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool TextJsonAndCsvContainSameSceneIds(SceneListResult result)
  {
    var text = BasicExporters.ToText(result);
    var json = BasicExporters.ToJson(result);
    var csv = BasicExporters.ToCsv(result);
    foreach (var row in result.Scenes)
    {
      if (!text.Contains(row.SceneId, StringComparison.Ordinal))
        return false;
      if (!json.Contains(row.SceneId, StringComparison.Ordinal))
        return false;
      if (!csv.Contains(row.SceneId, StringComparison.Ordinal))
        return false;
    }
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool JsonExportIsDeterministic(SceneListResult result) =>
      BasicExporters.ToJson(result) == BasicExporters.ToJson(result);

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool EmptyResultIsValidJson()
  {
    var json = BasicExporters.ToJson(new SceneListResult([]));
    using var document = JsonDocument.Parse(json);
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool CsvEmptyResultHasHeaderOnly()
  {
    var csv = BasicExporters.ToCsv(new SceneListResult([]));
    var lines = csv.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    return lines.Length == 1 && lines[0].StartsWith("SceneId", StringComparison.Ordinal);
  }
}
