using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class WorkspaceCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool InitCreatesWorkspaceFile()
    {
        var store = new TomlWorkspaceStore();
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            new WorkspaceCommands(store).InitAsync(path).GetAwaiter().GetResult();
            return File.Exists(path);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool InitThenOpenYieldsEmptyWorkspace()
    {
        var store = new TomlWorkspaceStore();
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            new WorkspaceCommands(store).InitAsync(path).GetAwaiter().GetResult();
            var workspace = new WorkspaceCommands(store).OpenAsync(path).GetAwaiter().GetResult();
            return workspace.Scenes.Count == 0 && workspace.Participants.Count == 0 && workspace.Locations.Count == 0;
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}