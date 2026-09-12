using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class LocationCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool LocationTimelineRowsAreOrderedByStart(NovelWorkspace workspace, LocationId location)
    {
        var rows = LocationQueries.GetLocationTimeline(workspace, location.Value).Rows;
        for (var index = 1; index < rows.Count; index++)
            if (rows[index].Start < rows[index - 1].Start)
                return false;
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool LocationTimelineEndsNotBeforeStarts(NovelWorkspace workspace, LocationId location)
    {
        var rows = LocationQueries.GetLocationTimeline(workspace, location.Value).Rows;
        foreach (var row in rows)
            if (row.End is { } end && end < row.Start)
                return false;
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DensityFirstUsePrecedesLastUse(NovelWorkspace workspace)
    {
        var rows = LocationQueries.GetLocationDensity(workspace).Rows;
        foreach (var row in rows)
            if (row.FirstUse is { } first && row.LastUse is { } last && first > last)
                return false;
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DensitySceneCountMatchesWorkspace(NovelWorkspace workspace)
    {
        var rows = LocationQueries.GetLocationDensity(workspace).Rows;
        foreach (var row in rows)
        {
            var expected = workspace.Scenes.Values.Count(scene =>
                scene.LocationId?.Value.Equals(row.LocationId, StringComparison.OrdinalIgnoreCase) == true);
            if (expected != row.SceneCount)
                return false;
        }
        return true;
    }
}
