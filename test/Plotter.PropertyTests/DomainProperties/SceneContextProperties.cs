using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.DomainProperties;

public sealed class SceneContextProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public Property FullSceneMetadataRoundTrips(NovelWorkspace workspace)
    {
        var store = new TomlWorkspaceStore();
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            store.SaveAsync(path, workspace).GetAwaiter().GetResult();
            var loaded = store.LoadAsync(path).GetAwaiter().GetResult();

            var original = workspace.Scenes.Values.OrderBy(s => s.Id.Value, StringComparer.Ordinal).ToArray();
            var reloaded = loaded.Scenes.Values.OrderBy(s => s.Id.Value, StringComparer.Ordinal).ToArray();
            if (original.Length != reloaded.Length)
                return false.ToProperty();

            for (var index = 0; index < original.Length; index++)
            {
                var before = original[index];
                var after = reloaded[index];
                if (before.Title != after.Title
                    || before.NarrativePosition != after.NarrativePosition
                    || before.Act != after.Act
                    || before.Chapter != after.Chapter
                    || before.StoryTime?.Date != after.StoryTime?.Date
                    || before.Duration?.Value != after.Duration?.Value
                    || before.LocationId?.Value != after.LocationId?.Value
                    || before.PovParticipantId?.Value != after.PovParticipantId?.Value
                    || before.Status != after.Status
                    || before.Notes != after.Notes
                    || !before.ParticipantIds.Select(p => p.Value).SequenceEqual(after.ParticipantIds.Select(p => p.Value))
                    || !before.Plots.Select(p => p.PlotId.Value).SequenceEqual(after.Plots.Select(p => p.PlotId.Value)))
                    return false.ToProperty();
            }

            return true.ToProperty();
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}