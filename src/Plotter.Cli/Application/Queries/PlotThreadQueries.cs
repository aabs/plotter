using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class PlotThreadQueries
{
    public static ThreadMatrixResult GetThreadMatrix(NovelWorkspace workspace)
    {
        var plotIds = workspace.Plots.Values
            .OrderBy(plot => plot.Id.Value, StringComparer.OrdinalIgnoreCase)
            .Select(plot => plot.Id.Value)
            .ToArray();

        var rows = workspace.Scenes.Values
            .OrderBy(scene => scene.NarrativePosition ?? int.MaxValue)
            .ThenBy(scene => scene.Id.Value, StringComparer.OrdinalIgnoreCase)
            .Select(scene =>
            {
                var cells = new Dictionary<string, ThreadMatrixCell>(StringComparer.OrdinalIgnoreCase);
                foreach (var plotId in plotIds)
                {
                    var relationship = scene.Plots.FirstOrDefault(plot => plot.PlotId.Value.Equals(plotId, StringComparison.OrdinalIgnoreCase));
                    cells[plotId] = relationship is null
                        ? new ThreadMatrixCell(PlotThreadClassification.Absent, null)
                        : new ThreadMatrixCell(relationship.Classification, relationship.Annotation);
                }
                return new ThreadMatrixRow(scene.Id.Value, cells);
            })
            .ToArray();

        return new ThreadMatrixResult(plotIds, rows);
    }

    public static ThreadTimelineResult GetThreadTimeline(NovelWorkspace workspace, string plotId)
    {
        var rows = workspace.Scenes.Values
            .Where(scene => scene.Plots.Any(plot => plot.PlotId.Value.Equals(plotId, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(scene => scene.NarrativePosition ?? int.MaxValue)
            .ThenBy(scene => scene.Id.Value, StringComparer.OrdinalIgnoreCase)
            .Select(scene =>
            {
                var relationship = scene.Plots.First(plot => plot.PlotId.Value.Equals(plotId, StringComparison.OrdinalIgnoreCase));
                return new ThreadTimelineRow(scene.Id.Value, scene.Title, relationship.Annotation);
            })
            .ToArray();

        return new ThreadTimelineResult(plotId, rows);
    }
}
