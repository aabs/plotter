using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Projections;
using Plotter.Cli.Application.Queries;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class ProjectionEquivalenceProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool TextJsonAndCsvContainSameSceneIds(SceneListResult result)
    {
        var projection = new SceneListProjection();
        var text = projection.ToText(result);
        var json = projection.ToJson(result);
        var csv = projection.ToCsv(result);
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
}