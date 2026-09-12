using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Auditing;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.ProjectionProperties;

public sealed class AuditProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool GapClassificationMatchesBoundaries(NonNegativeInt minutes)
    {
        var gap = TimeSpan.FromMinutes(minutes.Get);
        var expected = gap < TimeSpan.FromHours(6) ? GapClass.Normal
            : gap < TimeSpan.FromHours(24) ? GapClass.Long
            : GapClass.Overnight;
        return GapReportService.Classify(gap) == expected;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool TimeConfidenceAnnotationSuppressesNoDurationInfo(SceneId sceneId)
    {
        var workspace = new NovelWorkspace();
        workspace.Scenes[sceneId.Value] = new Scene(
            sceneId,
            "S",
            new StoryTime(new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc)),
            null,
            ContinuityAnnotations: [ContinuityAnnotation.TimeConfidence("approximate")]);
        var findings = new AuditService().RunTimeAudit(workspace);
        return findings.All(finding => finding.Code != "TIME-NO-DURATION");
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool MissingDurationWithoutAnnotationProducesInfo(SceneId sceneId)
    {
        var workspace = new NovelWorkspace();
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, "S", new StoryTime(new DateTime(2024, 1, 1, 8, 0, 0, DateTimeKind.Utc)));
        var findings = new AuditService().RunTimeAudit(workspace);
        return findings.Any(finding => finding.Code == "TIME-NO-DURATION" && finding.Severity == FindingSeverity.Info);
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool IntentionalOverlapIsNotError(DateTime start, SceneId sceneId1, SceneId sceneId2)
    {
        if (sceneId1.Value.Equals(sceneId2.Value, StringComparison.OrdinalIgnoreCase))
            return true;
        var participant = new ParticipantId("Mara");
        var firstLocation = new LocationId("L1");
        var secondLocation = new LocationId("L2");
        var duration = new SceneDuration(TimeSpan.FromMinutes(60));
        var scene1 = new Scene(sceneId1, "S1", new StoryTime(start), duration, firstLocation, [participant],
            ContinuityAnnotations: [ContinuityAnnotation.ContinuityStatus("intentional")]);
        var scene2 = new Scene(sceneId2, "S2", new StoryTime(start.AddMinutes(30)), duration, secondLocation, [participant]);

        var workspace = new NovelWorkspace();
        workspace.Participants[participant.Value] = new Participant(participant, "Mara");
        workspace.Locations[firstLocation.Value] = new Location(firstLocation, "L1");
        workspace.Locations[secondLocation.Value] = new Location(secondLocation, "L2");
        workspace.Scenes[sceneId1.Value] = scene1;
        workspace.Scenes[sceneId2.Value] = scene2;

        var findings = new AuditService().RunTimeAudit(workspace);
        return findings.All(finding => finding.Code != "TIME-OVERLAP" || finding.Severity != FindingSeverity.Error);
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool OverlapWithoutAnnotationIsError(DateTime start, SceneId sceneId1, SceneId sceneId2)
    {
        if (sceneId1.Value.Equals(sceneId2.Value, StringComparison.OrdinalIgnoreCase))
            return true;
        var participant = new ParticipantId("Mara");
        var firstLocation = new LocationId("L1");
        var secondLocation = new LocationId("L2");
        var duration = new SceneDuration(TimeSpan.FromMinutes(60));
        var scene1 = new Scene(sceneId1, "S1", new StoryTime(start), duration, firstLocation, [participant]);
        var scene2 = new Scene(sceneId2, "S2", new StoryTime(start.AddMinutes(30)), duration, secondLocation, [participant]);

        var workspace = new NovelWorkspace();
        workspace.Participants[participant.Value] = new Participant(participant, "Mara");
        workspace.Locations[firstLocation.Value] = new Location(firstLocation, "L1");
        workspace.Locations[secondLocation.Value] = new Location(secondLocation, "L2");
        workspace.Scenes[sceneId1.Value] = scene1;
        workspace.Scenes[sceneId2.Value] = scene2;

        var findings = new AuditService().RunTimeAudit(workspace);
        return findings.Any(finding => finding.Code == "TIME-OVERLAP" && finding.Severity == FindingSeverity.Error);
    }
}