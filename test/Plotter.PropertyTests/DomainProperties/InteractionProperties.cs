using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.DomainProperties;

public sealed class InteractionProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool AddInteractionWithUnassignedParticipantIsRejected(SceneId sceneId, ParticipantId participant)
  {
    var workspace = new NovelWorkspace();
    workspace.Scenes[sceneId.Value] = new Scene(sceneId);
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

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool AddInteractionWithAssignedParticipantSucceeds(SceneId sceneId, ParticipantId participant)
  {
    var workspace = new NovelWorkspace();
    workspace.Participants[participant.Value] = new Participant(participant, participant.Value);
    workspace.Scenes[sceneId.Value] = new Scene(sceneId, ParticipantIds: [participant]);
    var result = InteractionCommands.AddInteraction(workspace, sceneId.Value, "encounter", "Met", [participant.Value]);
    return result.Success && workspace.Scenes[sceneId.Value].Interactions.Count == 1;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool HistoryIsOrderedBySceneTime(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    foreach (var scene in workspace.Scenes.Values.Where(scene =>
        scene.ParticipantIds.Any(id => id.Value.Equals(participant, StringComparison.OrdinalIgnoreCase))))
      InteractionCommands.AddInteraction(workspace, scene.Id.Value, null, null, [participant]);

    var rows = InteractionQueries.GetParticipantHistory(workspace, participant).Rows;
    var times = rows.Select(row => workspace.Scenes[row.SceneId].StoryTime?.Date ?? DateTime.MaxValue).ToArray();
    for (var index = 1; index < times.Length; index++)
      if (times[index] < times[index - 1])
        return false;
    return true;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool RemoveInteractionReducesCount(SceneId sceneId, ParticipantId participant)
  {
    var workspace = new NovelWorkspace();
    workspace.Participants[participant.Value] = new Participant(participant, participant.Value);
    workspace.Scenes[sceneId.Value] = new Scene(sceneId, ParticipantIds: [participant]);
    InteractionCommands.AddInteraction(workspace, sceneId.Value, null, null, [participant.Value]);
    InteractionCommands.AddInteraction(workspace, sceneId.Value, null, null, [participant.Value]);
    var before = workspace.Scenes[sceneId.Value].Interactions.Count;
    InteractionCommands.RemoveInteraction(workspace, sceneId.Value, 0);
    return workspace.Scenes[sceneId.Value].Interactions.Count == before - 1;
  }
}
