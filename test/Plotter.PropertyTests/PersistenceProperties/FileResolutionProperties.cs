using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.PersistenceProperties;

public sealed class FileResolutionProperties
{
    private readonly NovelFileResolver _resolver = new();

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DefaultResolvesToCurrentFolderNovelToml()
    {
        var expected = Path.Combine(Directory.GetCurrentDirectory(), "novel.toml");
        return _resolver.Resolve(null) == expected;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ExplicitPathIsFullyQualified(SceneId path)
    {
        var resolved = _resolver.Resolve(path.Value);
        return Path.IsPathFullyQualified(resolved) && resolved == Path.GetFullPath(path.Value);
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DifferentExplicitPathsResolveIndependently(SceneId first, SceneId second)
    {
        if (Path.GetFullPath(first.Value) == Path.GetFullPath(second.Value))
            return true;
        return _resolver.Resolve(first.Value) != _resolver.Resolve(second.Value);
    }
}