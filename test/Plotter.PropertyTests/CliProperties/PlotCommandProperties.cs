using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class PlotCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool AddPlotRejectsWhitespaceId()
    {
        var workspace = new NovelWorkspace();
        try
        {
            PlotCommands.AddPlot(workspace, "   ");
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool AddPlotRejectsDuplicate(SceneId id)
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
    public bool AddPlotRejectsEndBeforeStart(SceneId id, DateTime start, DateTime end)
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
}