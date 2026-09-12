using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Travel;

namespace Plotter.Cli.Application.Queries;

public static class TravelQueries
{
    public static TravelResult GetTravel(NovelWorkspace workspace, string participantId, ITravelModelAdapter? model = null)
    {
        var scenes = workspace.Scenes.Values
            .Where(scene => scene.ParticipantIds.Any(participant => participant.Value.Equals(participantId, StringComparison.OrdinalIgnoreCase)))
            .Where(scene => scene.StoryTime?.Date is not null)
            .OrderBy(scene => scene.StoryTime!.Date!.Value)
            .ToArray();

        var steps = new List<TravelStep>();
        for (var index = 0; index < scenes.Length; index++)
        {
            var scene = scenes[index];
            TimeSpan? available = null;
            TimeSpan? modeled = null;

            if (index < scenes.Length - 1)
            {
                var next = scenes[index + 1];
                var currentEnd = scene.StoryTime!.Date!.Value + (scene.Duration?.Value ?? TimeSpan.Zero);
                available = next.StoryTime!.Date!.Value - currentEnd;
                if (scene.LocationId is { } from && next.LocationId is { } to && model is not null)
                    modeled = model.Estimate(from.Value, to.Value);
            }

            steps.Add(new TravelStep(scene.StoryTime!.Date!.Value, scene.LocationId?.Value, available, modeled));
        }

        return new TravelResult(participantId, steps);
    }
}