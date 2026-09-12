using FsCheck;
using FsCheck.Fluent;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.PropertyTests.Generators;

public static class DomainGenerators
{
    private static Gen<string> NonEmptyText() =>
        Gen.NonEmptyListOf(Gen.Choose(97, 122).Select(value => (char)value)).Select(chars => new string(chars.ToArray()));

    public static Arbitrary<SceneId> SceneIds() =>
        Arb.From(NonEmptyText().Select(value => new SceneId(value)));

    public static Arbitrary<ParticipantId> ParticipantIds() =>
        Arb.From(NonEmptyText().Select(value => new ParticipantId(value)));

    public static Arbitrary<LocationId> LocationIds() =>
        Arb.From(NonEmptyText().Select(value => new LocationId(value)));

    public static Arbitrary<PlotId> PlotIds() =>
        Arb.From(NonEmptyText().Select(value => new PlotId(value)));

    public static Arbitrary<SceneDuration> SceneDurations() =>
        Arb.From(Gen.Choose(0, 1000).Select(minutes => new SceneDuration(TimeSpan.FromMinutes(minutes))));

    public static Arbitrary<StoryTime> StoryTimes() =>
        Arb.From(Gen.Choose(0, 2_000_000_000).Select(ticks => new StoryTime(new DateTime(ticks, DateTimeKind.Utc))));

    public static Arbitrary<Scene> Scenes() =>
        Arb.From(from id in SceneIds().Generator
                 from title in NonEmptyText()
                 select new Scene(id, title));

    public static Arbitrary<SceneListResult> SceneListResults() =>
        Arb.From(Gen.ListOf(Scenes().Generator).Select(scenes => new SceneListResult(scenes.Select(scene => new SceneRow(
            scene.Id.Value,
            scene.Title,
            scene.StoryTime?.Date,
            scene.LocationId?.Value,
            scene.ParticipantIds.Select(participant => participant.Value).ToArray())).ToArray())));

    public static Arbitrary<NovelWorkspace> Workspaces() =>
        Arb.From(WorkspaceGenerator());

    private static Gen<NovelWorkspace> WorkspaceGenerator() =>
        from participantIds in Gen.ListOf(ParticipantIds().Generator)
        from locationIds in Gen.ListOf(LocationIds().Generator)
        from plotIds in Gen.ListOf(PlotIds().Generator)
        from sceneIds in Gen.ListOf(SceneIds().Generator)
        from stories in Gen.ListOf(StoryTimes().Generator)
        from durations in Gen.ListOf(SceneDurations().Generator)
        select BuildWorkspace(participantIds, locationIds, plotIds, sceneIds, stories, durations);

    private static NovelWorkspace BuildWorkspace(
        List<ParticipantId> participantIds,
        List<LocationId> locationIds,
        List<PlotId> plotIds,
        List<SceneId> sceneIds,
        List<StoryTime> stories,
        List<SceneDuration> durations)
    {
        var workspace = new NovelWorkspace();
        var participants = participantIds.Distinct().ToArray();
        var locations = locationIds.Distinct().ToArray();
        var plots = plotIds.Distinct().ToArray();
        var scenes = sceneIds.Distinct().ToArray();

        foreach (var id in participants)
            workspace.Participants[id.Value] = new Participant(id, id.Value);
        foreach (var id in locations)
            workspace.Locations[id.Value] = new Location(id, id.Value);
        foreach (var id in plots)
            workspace.Plots[id.Value] = new Plot(id, $"Plot {id.Value}");

        for (var index = 0; index < scenes.Length; index++)
        {
            var story = index < stories.Count ? stories[index] : null;
            SceneDuration? duration = index < durations.Count ? durations[index] : null;
            var sceneParticipants = participants.Length == 0 ? [] : participants.Take(index % (participants.Length + 1)).ToArray();
            var location = locations.Length == 0 ? null : (LocationId?)locations[index % locations.Length];
            var scenePlots = plots.Length == 0
                ? []
                : plots.Take(index % (plots.Length + 1)).Select(plot => new PlotRelationship(
                    plot,
                    (PlotThreadClassification)(index % 4),
                    index % 2 == 0 ? $"annotation-{index}" : null)).ToArray();
            var pov = sceneParticipants.Length == 0 ? null : (ParticipantId?)sceneParticipants[0];
            workspace.Scenes[scenes[index].Value] = new Scene(
                scenes[index],
                $"Scene {index}",
                story,
                duration,
                location,
                sceneParticipants,
                scenePlots,
                pov,
                "draft",
                $"Notes {index}",
                NarrativePosition: index,
                Act: $"Act {index / 3 + 1}",
                Chapter: $"Ch {index % 12 + 1}");
        }

        return workspace;
    }
}