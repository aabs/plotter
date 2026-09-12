using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class PlotThreadRenderers
{
    public static void RenderThreadMatrix(ThreadMatrixResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        writer.Write("Scene".PadRight(8));
        foreach (var header in result.PlotIds)
            writer.Write("  " + header.PadRight(14));
        writer.WriteLine();

        foreach (var row in result.Rows)
        {
            writer.Write(row.SceneId.PadRight(8));
            foreach (var plotId in result.PlotIds)
            {
                var symbol = row.Cells[plotId].Classification switch
                {
                    PlotThreadClassification.Primary => "●",
                    PlotThreadClassification.Secondary => "○",
                    PlotThreadClassification.Absent => "·",
                    _ => "?",
                };
                writer.Write("  " + symbol.PadRight(14));
            }
            writer.WriteLine();
        }
    }

    public static void RenderThreadTimeline(ThreadTimelineResult result, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        foreach (var row in result.Rows)
            writer.WriteLine($"{row.SceneId,-8} {row.Annotation ?? row.Title ?? string.Empty}");
    }
}
