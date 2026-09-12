using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public static class InteractionCommands
{
    public static CommandResult AddInteraction(NovelWorkspace workspace, string sceneId, string? type, string? description, IReadOnlyList<string> participantIds)
    {
        if (!workspace.Scenes.TryGetValue(sceneId, out var scene))
            throw new InvalidOperationException($"Scene '{sceneId}' does not exist.");

        var participants = participantIds.Select(id => new ParticipantId(id)).ToArray();
        foreach (var participant in participants)
            if (!scene.ParticipantIds.Any(id => id.Value.Equals(participant.Value, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Participant '{participant.Value}' is not assigned to scene '{sceneId}'.");

        var updated = scene with { Interactions = scene.Interactions.Append(new Interaction(type, description, participants)).ToArray() };
        workspace.Scenes[sceneId] = updated;
        return new CommandResult(true);
    }

    public static CommandResult RemoveInteraction(NovelWorkspace workspace, string sceneId, int index)
    {
        if (!workspace.Scenes.TryGetValue(sceneId, out var scene))
            throw new InvalidOperationException($"Scene '{sceneId}' does not exist.");
        if (index < 0 || index >= scene.Interactions.Count)
            throw new InvalidOperationException($"Interaction index {index} out of range for scene '{sceneId}'.");

        var updated = scene with { Interactions = scene.Interactions.Where((_, i) => i != index).ToArray() };
        workspace.Scenes[sceneId] = updated;
        return new CommandResult(true);
    }
}