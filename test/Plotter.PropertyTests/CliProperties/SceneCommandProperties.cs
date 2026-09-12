using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class SceneCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool InitializationWorkflowRoundTrips(SceneId sceneId, ParticipantId participant, LocationId location)
    {
        var store = new TomlWorkspaceStore();
        var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
        try
        {
            var workspaceCommands = new WorkspaceCommands(store);
            workspaceCommands.InitAsync(path).GetAwaiter().GetResult();
            var workspace = workspaceCommands.OpenAsync(path).GetAwaiter().GetResult();

            EntityCommands.AddParticipant(workspace, participant.Value);
            EntityCommands.AddLocation(workspace, location.Value);
            SceneCommands.AddScene(workspace, sceneId.Value);
            SceneCommands.SetScene(workspace, sceneId.Value, new SceneUpdate
            {
                StoryTime = new StoryTime(new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc)),
                ParticipantIds = [participant],
                LocationId = location,
            });
            workspaceCommands.SaveAsync(path, workspace).GetAwaiter().GetResult();

            var reloaded = workspaceCommands.OpenAsync(path).GetAwaiter().GetResult();
            if (!reloaded.Scenes.TryGetValue(sceneId.Value, out var scene))
                return false;
            return scene.LocationId?.Value == location.Value
                && scene.ParticipantIds.Any(participantId => participantId.Value == participant.Value)
                && scene.StoryTime?.Date == new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DuplicateSceneAddIsRejected(SceneId sceneId)
    {
        var workspace = new NovelWorkspace();
        SceneCommands.AddScene(workspace, sceneId.Value);
        try
        {
            SceneCommands.AddScene(workspace, sceneId.Value);
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }
}