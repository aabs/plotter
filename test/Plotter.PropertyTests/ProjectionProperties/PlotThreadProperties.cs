using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class PlotThreadProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool MatrixRowsCoverAllScenes(NovelWorkspace workspace)
    {
        var result = PlotThreadQueries.GetThreadMatrix(workspace);
        var expected = workspace.Scenes.Values.Select(scene => scene.Id.Value).OrderBy(id => id, StringComparer.OrdinalIgnoreCase).ToArray();
        var actual = result.Rows.Select(row => row.SceneId).OrderBy(id => id, StringComparer.OrdinalIgnoreCase).ToArray();
        return expected.SequenceEqual(actual);
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool MatrixColumnsCoverAllPlots(NovelWorkspace workspace)
    {
        var result = PlotThreadQueries.GetThreadMatrix(workspace);
        var expected = workspace.Plots.Values.Select(plot => plot.Id.Value).OrderBy(id => id, StringComparer.OrdinalIgnoreCase).ToArray();
        return expected.SequenceEqual(result.PlotIds);
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool MatrixCellMatchesRelationship(NovelWorkspace workspace)
    {
        var result = PlotThreadQueries.GetThreadMatrix(workspace);
        foreach (var row in result.Rows)
        {
            var scene = workspace.Scenes[row.SceneId];
            foreach (var (plotId, cell) in row.Cells)
            {
                var relationship = scene.Plots.FirstOrDefault(plot => plot.PlotId.Value.Equals(plotId, StringComparison.OrdinalIgnoreCase));
                if (relationship is null)
                {
                    if (cell.Classification != PlotThreadClassification.Absent)
                        return false;
                }
                else if (cell.Classification != relationship.Classification || cell.Annotation != relationship.Annotation)
                    return false;
            }
        }
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ThreadTimelineRowsAreOrderedByNarrativePosition(NovelWorkspace workspace)
    {
        var plot = workspace.Plots.Keys.FirstOrDefault();
        if (plot is null)
            return true;
        var positions = workspace.Scenes.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.NarrativePosition ?? int.MaxValue,
            StringComparer.OrdinalIgnoreCase);
        var rows = PlotThreadQueries.GetThreadTimeline(workspace, plot).Rows;
        for (var index = 1; index < rows.Count; index++)
            if (positions[rows[index].SceneId] < positions[rows[index - 1].SceneId])
                return false;
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool PlotWithNoScenesYieldsEmptyTimeline(NovelWorkspace workspace)
    {
        var plot = workspace.Plots.Keys.FirstOrDefault();
        if (plot is null)
            return true;
        var referenced = workspace.Scenes.Values.Any(scene => scene.Plots.Any(relationship => relationship.PlotId.Value.Equals(plot, StringComparison.OrdinalIgnoreCase)));
        var rows = PlotThreadQueries.GetThreadTimeline(workspace, plot).Rows;
        return referenced ? rows.Count > 0 : rows.Count == 0;
    }
}