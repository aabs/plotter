using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class InteractionQueries
{
    public static InteractionHistoryResult GetParticipantHistory(NovelWorkspace workspace, string participantId)
    {
        var rows = workspace.Scenes.Values
            .SelectMany(scene => scene.Interactions.Select(interaction => (Scene: scene, Interaction: interaction)))
            .Where(item => item.Interaction.ParticipantIds.Any(participant => participant.Value.Equals(participantId, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(item => item.Scene.StoryTime?.Date ?? DateTime.MaxValue)
            .ThenBy(item => item.Scene.Id.Value, StringComparer.OrdinalIgnoreCase)
            .Select(item => new InteractionHistoryRow(
                item.Scene.Id.Value,
                item.Interaction.Type,
                item.Interaction.Description,
                item.Interaction.ParticipantIds.Select(participant => participant.Value).ToArray()))
            .ToArray();

        return new InteractionHistoryResult(participantId, rows);
    }
}