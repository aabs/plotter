using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;

namespace Plotter.PropertyTests.Properties;

public sealed class WorkspaceProperties
{
    [Property]
    public bool NonEmptyIdsArePreserved(NonEmptyString id)
    {
        var value = Validation.RequiredId(id.Get, "ID");
        return value.Length > 0;
    }
}
