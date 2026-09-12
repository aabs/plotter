using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.PropertyTests.Generators;
using Plotter.PropertyTests.Oracles;

namespace Plotter.PropertyTests.PersistenceProperties;

public sealed class WorkspaceRecoveryProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool CopiedWorkspaceReopensEquivalently(NovelWorkspace workspace)
    {
        var store = new TomlWorkspaceStore();
        var pathA = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        var pathB = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            store.SaveAsync(pathA, workspace).GetAwaiter().GetResult();
            File.Copy(pathA, pathB);
            var loaded = store.LoadAsync(pathB).GetAwaiter().GetResult();
            return ReferenceOracles.CanonicalText(loaded) == ReferenceOracles.CanonicalText(workspace);
        }
        finally
        {
            if (File.Exists(pathA)) File.Delete(pathA);
            if (File.Exists(pathB)) File.Delete(pathB);
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool MalformedInputIsRejectedAndPreserved(SceneId name)
    {
        var store = new TomlWorkspaceStore();
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            var original = "this is not toml {{{";
            File.WriteAllText(path, original);
            try
            {
                store.LoadAsync(path).GetAwaiter().GetResult();
                return false;
            }
            catch (InvalidDataException)
            {
                // expected
            }
            return File.ReadAllText(path) == original;
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool IncompatibleVersionIsRejectedAndPreserved(SceneId version)
    {
        var store = new TomlWorkspaceStore();
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            var content = $"format_version = \"{version.Value}\"\n";
            File.WriteAllText(path, content);
            try
            {
                store.LoadAsync(path).GetAwaiter().GetResult();
                return false;
            }
            catch (InvalidDataException)
            {
                // expected
            }
            return File.ReadAllText(path) == content;
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool CompatibilityValidationReportsMalformedInput(SceneId name)
    {
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            File.WriteAllText(path, "not toml at all");
            var message = WorkspaceCompatibilityService.Validate(path);
            return message is not null && message.StartsWith("Malformed", StringComparison.Ordinal);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}