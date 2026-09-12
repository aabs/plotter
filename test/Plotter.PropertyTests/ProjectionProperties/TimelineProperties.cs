using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class TimelineProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ChronologicalOrderingIsNonDecreasing(NovelWorkspace workspace)
    {
        var rows = TimelineQueries.GetChronological(workspace, new SceneListQuery()).Scenes;
        for (var index = 1; index < rows.Count; index++)
        {
            var previous = rows[index - 1].StoryDateTime ?? DateTime.MaxValue;
            var current = rows[index].StoryDateTime ?? DateTime.MaxValue;
            if (current < previous)
                return false;
            if (current == previous
                && string.Compare(rows[index - 1].SceneId, rows[index].SceneId, StringComparison.OrdinalIgnoreCase) > 0)
                return false;
        }
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool InclusiveRangeIncludesAllDatedScenesInWindow(NovelWorkspace workspace)
    {
        var dates = workspace.Scenes.Values
            .Select(scene => scene.StoryTime?.Date)
            .Where(date => date is not null)
            .Select(date => date!.Value.Date)
            .ToArray();
        if (dates.Length == 0)
            return true;

        var from = dates.Min();
        var to = dates.Max();
        var result = TimelineQueries.GetChronological(workspace, new SceneListQuery(
            From: from.ToString("yyyy-MM-dd"),
            To: to.ToString("yyyy-MM-dd")));

        var expected = dates.Count(date => date >= from && date < to.AddDays(1));
        return result.Scenes.Count == expected;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool UndatedScenesArePreservedWithoutRange(NovelWorkspace workspace)
    {
        var undated = workspace.Scenes.Values.Count(scene => scene.StoryTime?.Date is null);
        var result = TimelineQueries.GetChronological(workspace, new SceneListQuery());
        var undatedInResult = result.Scenes.Count(row => row.StoryDateTime is null);
        return undatedInResult == undated;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool RangeExcludesUndatedScenes(NovelWorkspace workspace, DateTime day)
    {
        var result = TimelineQueries.GetChronological(workspace, new SceneListQuery(
            From: day.Date.ToString("yyyy-MM-dd"),
            To: day.Date.ToString("yyyy-MM-dd")));
        return result.Scenes.All(row => row.StoryDateTime is not null);
    }
}
