using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class LocationOccupancyProperties
{
    private static NovelWorkspace WorkspaceWith(params Scene[] scenes)
    {
        var workspace = new NovelWorkspace();
        foreach (var scene in scenes)
        {
            workspace.Scenes[scene.Id.Value] = scene;
            if (scene.LocationId is { } location)
                workspace.Locations[location.Value] = new Location(location, location.Value);
            foreach (var participant in scene.ParticipantIds)
                workspace.Participants[participant.Value] = new Participant(participant, participant.Value);
        }
        return workspace;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ExplicitDurationDefinesOccupancyWindow(DateTime start, NonNegativeInt minutes)
    {
        var participant = new ParticipantId("Mara");
        var location = new LocationId("Boarding house");
        var duration = TimeSpan.FromMinutes(minutes.Get + 1);
        var scene = new Scene(new SceneId("S1"), "S1", new StoryTime(start), new SceneDuration(duration), location, [participant]);
        var workspace = WorkspaceWith(scene);

        var within = LocationQueries.GetActiveLocation(workspace, participant.Value, start.AddMinutes(duration.TotalMinutes / 2));
        var after = LocationQueries.GetActiveLocation(workspace, participant.Value, start.Add(duration));
        return within.LocationId == location.Value && after.LocationId is null;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DurationLessOccupancyEndsAtNextScene(DateTime first, DateTime second)
    {
        if (second <= first)
            return true;
        var participant = new ParticipantId("Mara");
        var firstLocation = new LocationId("Boarding house");
        var secondLocation = new LocationId("Station");
        var scene1 = new Scene(new SceneId("S1"), "S1", new StoryTime(first), null, firstLocation, [participant]);
        var scene2 = new Scene(new SceneId("S2"), "S2", new StoryTime(second), null, secondLocation, [participant]);
        var workspace = WorkspaceWith(scene1, scene2);

        var beforeNext = LocationQueries.GetActiveLocation(workspace, participant.Value, first.AddMinutes((second - first).TotalMinutes / 2));
        var atNext = LocationQueries.GetActiveLocation(workspace, participant.Value, second);
        return beforeNext.LocationId == firstLocation.Value && atNext.LocationId == secondLocation.Value;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ParticipantWithoutSceneHasNoLocation(DateTime at)
    {
        var participant = new ParticipantId("Mara");
        var workspace = new NovelWorkspace();
        workspace.Participants[participant.Value] = new Participant(participant, "Mara");
        var result = LocationQueries.GetActiveLocation(workspace, participant.Value, at);
        return result.LocationId is null && result.ConflictLocationIds.Count == 0;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool SameTimeDifferentLocationsReportConflict(DateTime start, NonNegativeInt minutes)
    {
        var participant = new ParticipantId("Mara");
        var firstLocation = new LocationId("Boarding house");
        var secondLocation = new LocationId("Station");
        var duration = new SceneDuration(TimeSpan.FromMinutes(minutes.Get + 1));
        var scene1 = new Scene(new SceneId("S1"), "S1", new StoryTime(start), duration, firstLocation, [participant]);
        var scene2 = new Scene(new SceneId("S2"), "S2", new StoryTime(start), duration, secondLocation, [participant]);
        var workspace = WorkspaceWith(scene1, scene2);

        var result = LocationQueries.GetActiveLocation(workspace, participant.Value, start);
        return result.ConflictLocationIds.Count == 2;
    }
}
