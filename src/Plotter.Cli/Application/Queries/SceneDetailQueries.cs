using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class SceneDetailQueries
{
    public static SceneCardDetail? GetSceneDetail(NovelWorkspace workspace, string sceneId)
    {
        if (!workspace.Scenes.TryGetValue(sceneId, out var scene))
            return null;

        var dated = workspace.Scenes.Values
            .Where(candidate => candidate.StoryTime?.Date is not null)
            .OrderBy(candidate => candidate.StoryTime!.Date!.Value)
            .ToArray();
        var index = Array.FindIndex(dated, candidate => candidate.Id.Value.Equals(sceneId, StringComparison.OrdinalIgnoreCase));

        SceneCardNeighbor? previous = null;
        SceneCardNeighbor? next = null;
        if (index >= 0)
        {
            if (index > 0)
                previous = ToNeighbor(dated[index - 1]);
            if (index < dated.Length - 1)
                next = ToNeighbor(dated[index + 1]);
        }

        TimeSpan? elapsed = scene.StoryTime?.Date is { } start && previous?.StoryDateTime is { } prevStart
            ? start - prevStart
            : null;

        return new SceneCardDetail(
            scene.Id,
            scene.Title,
            scene.NarrativePosition,
            scene.Act,
            scene.Chapter,
            scene.StoryTime,
            scene.Duration,
            scene.LocationId,
            scene.ParticipantIds,
            scene.Plots,
            scene.PovParticipantId,
            scene.Status,
            scene.Notes,
            previous,
            next,
            elapsed,
            BuildFlags(workspace, scene, previous));
    }

    private static SceneCardNeighbor ToNeighbor(Scene scene) =>
        new(scene.Id.Value, scene.Title, scene.LocationId?.Value, scene.StoryTime?.Date);

    private static List<string> BuildFlags(NovelWorkspace workspace, Scene scene, SceneCardNeighbor? previous)
    {
        var flags = new List<string>();
        if (previous is null || !workspace.Scenes.TryGetValue(previous.SceneId, out var previousScene))
            return flags;

        foreach (var participant in scene.ParticipantIds)
        {
            var wasPresent = previousScene.ParticipantIds.Any(id => id.Value.Equals(participant.Value, StringComparison.OrdinalIgnoreCase));
            if (!wasPresent)
            {
                flags.Add($"{Name(workspace.Participants, participant.Value)} was not in the previous scene ({previous.SceneId})");
            }
            else if (previousScene.LocationId is { } previousLocation && scene.LocationId is { } currentLocation
                && !previousLocation.Value.Equals(currentLocation.Value, StringComparison.OrdinalIgnoreCase))
            {
                flags.Add($"{Name(workspace.Participants, participant.Value)} has no recorded travel from {Name(workspace.Locations, previousLocation.Value)} to {Name(workspace.Locations, currentLocation.Value)}");
            }
        }

        return flags;
    }

    private static string Name(IReadOnlyDictionary<string, Participant> participants, string id) =>
        participants.GetValueOrDefault(id)?.Name ?? id;

    private static string Name(IReadOnlyDictionary<string, Location> locations, string id) =>
        locations.GetValueOrDefault(id)?.Name ?? id;
}
