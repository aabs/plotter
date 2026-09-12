using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class InteractionCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool RejectsParticipantNotAssignedToScene(SceneId sceneId, ParticipantId missing, ParticipantId present)
    {
        if (missing.Value.Equals(present.Value, StringComparison.OrdinalIgnoreCase))
            return true;
        var workspace = new NovelWorkspace();
        workspace.Participants[present.Value] = new Participant(present, present.Value);
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, ParticipantIds: [present]);
        try
        {
            InteractionCommands.AddInteraction(workspace, sceneId.Value, null, null, [missing.Value]);
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool MissingSceneIsRejected(SceneId sceneId, ParticipantId participant)
    {
        var workspace = new NovelWorkspace();
        try
        {
            InteractionCommands.AddInteraction(workspace, sceneId.Value, null, null, [participant.Value]);
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }
}
