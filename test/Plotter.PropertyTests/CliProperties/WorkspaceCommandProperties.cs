using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;
using Plotter.PropertyTests.Oracles;

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
  public bool CopyReopenOfflinePreservesWorkspace(NovelWorkspace workspace)
  {
    var store = new TomlWorkspaceStore();
    var pathA = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
    var pathB = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
    try
    {
      new WorkspaceCommands(store).SaveAsync(pathA, workspace).GetAwaiter().GetResult();
      File.Copy(pathA, pathB);
      var loaded = new WorkspaceCommands(store).OpenAsync(pathB).GetAwaiter().GetResult();
      return ReferenceOracles.CanonicalText(loaded) == ReferenceOracles.CanonicalText(workspace);
    }
    finally
    {
      if (File.Exists(pathA)) File.Delete(pathA);
      if (File.Exists(pathB)) File.Delete(pathB);
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ExplicitFileSelectionParses(SceneId fileName)
  {
    var (options, remaining) = FileSelectionParser.Parse(["--file", fileName.Value, "init"]);
    return options.ExplicitPath == fileName.Value && remaining.SequenceEqual(["init"]);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool DefaultFileSelectionHasNoPath()
  {
    var (options, remaining) = FileSelectionParser.Parse(["timeline"]);
    return options.ExplicitPath is null && remaining.SequenceEqual(["timeline"]);
  }
}
