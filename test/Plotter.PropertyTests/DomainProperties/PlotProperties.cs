using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.DomainProperties;

public sealed class PlotProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool EmptyPlotIdIsRejected()
    {
        try
        {
            _ = new PlotId("   ");
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DuplicatePlotIdIsRejected(SceneId id)
    {
        var workspace = new NovelWorkspace();
        PlotCommands.AddPlot(workspace, id.Value);
        try
        {
            PlotCommands.AddPlot(workspace, id.Value);
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool EndBeforeStartIsRejected(SceneId id, DateTime start, DateTime end)
    {
        if (end >= start)
            return true;
        var workspace = new NovelWorkspace();
        try
        {
            PlotCommands.AddPlot(workspace, id.Value, null, new StoryTime(start), new StoryTime(end));
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ValidBoundsAreAccepted(SceneId id, DateTime start, DateTime end)
    {
        if (end < start)
            return true;
        var workspace = new NovelWorkspace();
        var result = PlotCommands.AddPlot(workspace, id.Value, null, new StoryTime(start), new StoryTime(end));
        return result.Success && workspace.Plots.ContainsKey(id.Value);
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool RemovePlotClearsSceneReferences(SceneId plotId, SceneId sceneId)
    {
        var workspace = new NovelWorkspace();
        PlotCommands.AddPlot(workspace, plotId.Value);
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, Plots: [new PlotRelationship(new PlotId(plotId.Value), PlotThreadClassification.Primary)]);
        PlotCommands.RemovePlot(workspace, plotId.Value);
        return workspace.Scenes[sceneId.Value].Plots.Count == 0;
    }
}