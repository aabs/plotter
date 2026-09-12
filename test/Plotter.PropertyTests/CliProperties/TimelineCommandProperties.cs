using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class TimelineCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool InvertedRangeReturnsNoScenes(NovelWorkspace workspace, DateTime from, DateTime to)
    {
        if (from.Date >= to.Date)
            return true;
        var result = TimelineQueries.GetChronological(workspace, new SceneListQuery(
            From: to.Date.ToString("yyyy-MM-dd"),
            To: from.Date.ToString("yyyy-MM-dd")));
        return result.Scenes.Count == 0;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool IdenticalDateRangeReturnsOnlyThatDate(NovelWorkspace workspace, DateTime day)
    {
        var result = TimelineQueries.GetChronological(workspace, new SceneListQuery(
            From: day.Date.ToString("yyyy-MM-dd"),
            To: day.Date.ToString("yyyy-MM-dd")));
        foreach (var row in result.Scenes)
        {
            if (row.StoryDateTime is not { } date || date < day.Date || date >= day.Date.AddDays(1))
                return false;
        }
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool FlashbackRowsRenderMarker(NovelWorkspace workspace)
    {
        var result = TimelineQueries.GetManuscript(workspace);
        var writer = new StringWriter();
        TimelineRenderers.RenderManuscript(result, writer);
        var output = writer.ToString();
        var flashbackCount = result.Rows.Count(row => row.IsFlashback);
        var markerCount = output.Split("[FLASHBACK]").Length - 1;
        return markerCount == flashbackCount;
    }
}
