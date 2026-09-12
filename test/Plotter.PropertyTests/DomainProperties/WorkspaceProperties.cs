using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.DomainProperties;

public sealed class WorkspaceProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool NonEmptyIdsArePreserved(SceneId id)
    {
        var value = Validation.RequiredId(id.Value, "ID");
        return value.Length > 0 && value == id.Value;
    }
}
