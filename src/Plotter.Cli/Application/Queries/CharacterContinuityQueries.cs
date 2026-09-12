using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class CharacterContinuityQueries
{
    public static ItineraryResult GetItinerary(NovelWorkspace workspace, string participantId)
    {
        var rows = workspace.Scenes.Values
            .Where(scene => scene.ParticipantIds.Any(participant => participant.Value.Equals(participantId, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(scene => scene.StoryTime?.Date ?? DateTime.MaxValue)
            .ThenBy(scene => scene.Id.Value, StringComparer.OrdinalIgnoreCase)
            .Select(scene => new ItineraryRow(scene.StoryTime?.Date, scene.LocationId?.Value, scene.Id.Value, scene.Title))
            .ToArray();
        return new ItineraryResult(participantId, rows);
    }

    public static LaneResult GetLanes(NovelWorkspace workspace, IReadOnlyList<string> participantIds, DateTime? date = null, string? locationId = null)
    {
        var participants = participantIds.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        var occurrences = new List<(DateTime? Time, string Participant, string SceneId, string? LocationId)>();
        foreach (var scene in workspace.Scenes.Values)
        {
            if (date is not null && scene.StoryTime?.Date is { } sceneDate
                && (sceneDate < date.Value.Date || sceneDate >= date.Value.Date.AddDays(1)))
                continue;
            if (locationId is not null
                && (scene.LocationId?.Value is not { } actualLocation || !actualLocation.Equals(locationId, StringComparison.OrdinalIgnoreCase)))
                continue;

            foreach (var participant in scene.ParticipantIds)
                if (participants.Any(selected => selected.Equals(participant.Value, StringComparison.OrdinalIgnoreCase)))
                    occurrences.Add((scene.StoryTime?.Date, participant.Value, scene.Id.Value, scene.LocationId?.Value));
        }

        var rows = occurrences
            .GroupBy(occurrence => occurrence.Time)
            .OrderBy(group => group.Key ?? DateTime.MaxValue)
            .Select(group => new LaneRow(group.Key, participants.Select(participant =>
            {
                var cells = group
                    .Where(occurrence => occurrence.Participant.Equals(participant, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(occurrence => occurrence.SceneId, StringComparer.OrdinalIgnoreCase)
                    .Select(occurrence => new LaneOccurrence(occurrence.SceneId, occurrence.LocationId))
                    .ToArray();
                return new LaneCell(cells);
            }).ToArray()))
            .ToArray();

        return new LaneResult(participants, rows);
    }

    public static IReadOnlyList<string> ResolveGroup(NovelWorkspace workspace, string groupName) =>
        workspace.ParticipantGroups.Values
            .Where(group => group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
            .SelectMany(group => group.ParticipantIds.Select(participant => participant.Value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}