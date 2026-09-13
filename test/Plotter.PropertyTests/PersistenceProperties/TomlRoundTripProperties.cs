using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.PropertyTests.Generators;
using Plotter.PropertyTests.Oracles;

namespace Plotter.PropertyTests.PersistenceProperties;

public sealed class TomlRoundTripProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public Property SaveLoadSaveIsIdempotent(NovelWorkspace workspace)
  {
    var store = new TomlWorkspaceStore();
    var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
    try
    {
      store.SaveAsync(path, workspace).GetAwaiter().GetResult();
      var first = File.ReadAllText(path);
      var loaded = store.LoadAsync(path).GetAwaiter().GetResult();
      store.SaveAsync(path, loaded).GetAwaiter().GetResult();
      var second = File.ReadAllText(path);
      return (first == second).ToProperty();
    }
    finally
    {
      if (File.Exists(path))
        File.Delete(path);
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public Property LoadedWorkspaceMatchesCanonicalOracle(NovelWorkspace workspace)
  {
    var store = new TomlWorkspaceStore();
    var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
    try
    {
      store.SaveAsync(path, workspace).GetAwaiter().GetResult();
      var loaded = store.LoadAsync(path).GetAwaiter().GetResult();
      return (ReferenceOracles.CanonicalText(loaded) == ReferenceOracles.CanonicalText(workspace)).ToProperty();
    }
    finally
    {
      if (File.Exists(path))
        File.Delete(path);
    }
  }
}
