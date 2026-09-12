using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class PlotThreadCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool AllClassificationSymbolsRender(SceneId sceneId)
    {
        var workspace = new NovelWorkspace();
        var plotIds = new[] { "P1", "P2", "P3", "P4" };
        foreach (var plotId in plotIds)
            workspace.Plots[plotId] = new Plot(new PlotId(plotId), plotId);

        workspace.Scenes[sceneId.Value] = new Scene(sceneId, Plots:
        [
            new PlotRelationship(new PlotId("P1"), PlotThreadClassification.Primary),
            new PlotRelationship(new PlotId("P2"), PlotThreadClassification.Secondary),
            new PlotRelationship(new PlotId("P3"), PlotThreadClassification.Absent),
            new PlotRelationship(new PlotId("P4"), PlotThreadClassification.NotClassified),
        ]);

        var result = PlotThreadQueries.GetThreadMatrix(workspace);
        var writer = new StringWriter();
        PlotThreadRenderers.RenderThreadMatrix(result, writer);
        var output = writer.ToString();
        return output.Contains('●') && output.Contains('○') && output.Contains('·') && output.Contains('?');
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ThreadTimelineRendersAnnotations(NovelWorkspace workspace)
    {
        var plot = workspace.Plots.Keys.FirstOrDefault();
        if (plot is null)
            return true;
        var result = PlotThreadQueries.GetThreadTimeline(workspace, plot);
        var writer = new StringWriter();
        PlotThreadRenderers.RenderThreadTimeline(result, writer);
        var output = writer.ToString();
        foreach (var row in result.Rows)
        {
            var text = row.Annotation ?? row.Title ?? string.Empty;
            if (!output.Contains(row.SceneId, StringComparison.Ordinal) || (text.Length > 0 && !output.Contains(text, StringComparison.Ordinal)))
                return false;
        }
        return true;
    }
}
