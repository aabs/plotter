using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class ComposableCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ManuscriptViewsAreDeterministic(NovelWorkspace workspace)
    {
        var first = TimelineQueries.GetManuscript(workspace);
        var second = TimelineQueries.GetManuscript(workspace);
        return first.Rows.Select(row => row.SceneId).SequenceEqual(second.Rows.Select(row => row.SceneId));
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool CurrentFolderSelectionResolvesNovelToml()
    {
        var resolver = new NovelFileResolver();
        return resolver.Resolve(null) == Path.Combine(Directory.GetCurrentDirectory(), "novel.toml");
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool EmptyRangeResultIsValid(NovelWorkspace workspace)
    {
        var result = TimelineQueries.GetChronological(workspace, new SceneListQuery(From: "2030-01-01", To: "2020-01-01"));
        return result.Scenes.Count == 0;
    }
}
