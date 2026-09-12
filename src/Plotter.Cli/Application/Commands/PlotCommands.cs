using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public static class PlotCommands
{
    public static CommandResult AddPlot(NovelWorkspace workspace, string id, string? description = null, StoryTime? start = null, StoryTime? end = null)
    {
        var plotId = new PlotId(Validation.RequiredId(id, "Plot ID"));
        Validation.EnsureUnique(workspace.Plots, plotId.Value, "Plot");
        var plot = new Plot(plotId, description, start, end);
        ValidateBounds(plot);
        workspace.Plots[plotId.Value] = plot;
        return new CommandResult(true);
    }

    public static CommandResult SetPlot(NovelWorkspace workspace, string id, string? description = null, StoryTime? start = null, StoryTime? end = null)
    {
        if (!workspace.Plots.TryGetValue(id, out var plot))
            throw new InvalidOperationException($"Plot '{id}' does not exist.");

        var updated = plot with
        {
            Description = description ?? plot.Description,
            StartTime = start ?? plot.StartTime,
            EndTime = end ?? plot.EndTime,
        };
        ValidateBounds(updated);
        workspace.Plots[id] = updated;
        return new CommandResult(true);
    }

    public static CommandResult RemovePlot(NovelWorkspace workspace, string id)
    {
        if (!workspace.Plots.Remove(id))
            throw new InvalidOperationException($"Plot '{id}' does not exist.");

        foreach (var sceneId in workspace.Scenes.Keys.ToList())
        {
            var scene = workspace.Scenes[sceneId];
            var remaining = scene.Plots.Where(plot => !plot.PlotId.Value.Equals(id, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (remaining.Length != scene.Plots.Count)
                workspace.Scenes[sceneId] = scene with { Plots = remaining };
        }

        return new CommandResult(true);
    }

    private static void ValidateBounds(Plot plot)
    {
        if (plot.StartTime is { } start && plot.EndTime is { } end && end.Date < start.Date)
            throw new InvalidOperationException($"Plot '{plot.Id}' end precedes its start.");
    }
}
