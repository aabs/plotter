using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public sealed class AuditService : IAuditService
{
    public IReadOnlyList<AuditFinding> RunTimeAudit(NovelWorkspace workspace)
    {
        var findings = new List<AuditFinding>();
        foreach (var scene in workspace.Scenes.Values)
        {
            if (scene.StoryTime?.Date is { } date && date.TimeOfDay == TimeSpan.Zero && !HasAnnotation(scene, "time-confidence"))
                findings.Add(new AuditFinding(FindingSeverity.Warning, "TIME-NO-TIME", $"Scene '{scene.Id}' has a date but no time.", [scene.Id.Value]));
            if (scene.Duration is null && !HasAnnotation(scene, "time-confidence"))
                findings.Add(new AuditFinding(FindingSeverity.Info, "TIME-NO-DURATION", $"Scene '{scene.Id}' has no duration.", [scene.Id.Value]));
        }
        findings.AddRange(FindOverlaps(workspace));
        return findings;
    }

    public IReadOnlyList<AuditFinding> RunTravelAudit(NovelWorkspace workspace)
    {
        var findings = new List<AuditFinding>();
        foreach (var participant in workspace.Participants.Values)
        {
            var scenes = workspace.Scenes.Values
                .Where(scene => scene.ParticipantIds.Any(id => id.Value.Equals(participant.Id.Value, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(scene => scene.StoryTime?.Date ?? DateTime.MaxValue)
                .ToArray();
            for (var index = 1; index < scenes.Length; index++)
            {
                var previous = scenes[index - 1];
                var current = scenes[index];
                if (previous.LocationId is { } from && current.LocationId is { } to
                    && !from.Value.Equals(to.Value, StringComparison.OrdinalIgnoreCase)
                    && !HasAnnotation(current, "travel-status"))
                    findings.Add(new AuditFinding(
                        FindingSeverity.Warning,
                        "TRAVEL-NO-ROUTE",
                        $"{participant.Name} moves from '{from.Value}' to '{to.Value}' without a recorded travel status.",
                        [previous.Id.Value, current.Id.Value]));
            }
        }
        return findings;
    }

    public IReadOnlyList<AuditFinding> RunParticipantAudit(NovelWorkspace workspace) => FindOverlaps(workspace);

    public IReadOnlyList<AuditFinding> RunLocationAudit(NovelWorkspace workspace)
    {
        var findings = new List<AuditFinding>();
        foreach (var location in workspace.Locations.Values)
        {
            var used = workspace.Scenes.Values.Any(scene =>
                scene.LocationId?.Value.Equals(location.Id.Value, StringComparison.OrdinalIgnoreCase) == true);
            if (!used)
                findings.Add(new AuditFinding(FindingSeverity.Info, "LOC-UNUSED", $"Location '{location.Name}' has no scenes.", null));
        }
        return findings;
    }

    public IReadOnlyList<AuditFinding> RunAll(NovelWorkspace workspace) =>
        RunTimeAudit(workspace)
            .Concat(RunTravelAudit(workspace))
            .Concat(RunParticipantAudit(workspace))
            .Concat(RunLocationAudit(workspace))
            .ToArray();

    private static List<AuditFinding> FindOverlaps(NovelWorkspace workspace)
    {
        var findings = new List<AuditFinding>();
        foreach (var participant in workspace.Participants.Values)
        {
            var scenes = workspace.Scenes.Values
                .Where(scene => scene.ParticipantIds.Any(id => id.Value.Equals(participant.Id.Value, StringComparison.OrdinalIgnoreCase)))
                .Where(scene => scene.StoryTime?.Date is not null)
                .ToArray();
            for (var first = 0; first < scenes.Length; first++)
            {
                for (var second = first + 1; second < scenes.Length; second++)
                {
                    var a = scenes[first];
                    var b = scenes[second];
                    if (a.Id.Value.Equals(b.Id.Value, StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (a.LocationId is null || b.LocationId is null
                        || a.LocationId.Value.Value.Equals(b.LocationId.Value.Value, StringComparison.OrdinalIgnoreCase))
                        continue;
                    if (!Overlaps(a, b))
                        continue;
                    if (HasAnnotation(a, "continuity-status") || HasAnnotation(b, "continuity-status"))
                        continue;
                    findings.Add(new AuditFinding(
                        FindingSeverity.Error,
                        "TIME-OVERLAP",
                        $"{a.Id.Value} and {b.Id.Value} overlap, but {participant.Name} appears in both locations.",
                        [a.Id.Value, b.Id.Value]));
                }
            }
        }
        return findings;
    }

    private static bool Overlaps(Scene a, Scene b)
    {
        var aStart = a.StoryTime!.Date!.Value;
        var aEnd = aStart + (a.Duration?.Value ?? TimeSpan.Zero);
        var bStart = b.StoryTime!.Date!.Value;
        var bEnd = bStart + (b.Duration?.Value ?? TimeSpan.Zero);
        return aStart < bEnd && bStart < aEnd;
    }

    private static bool HasAnnotation(Scene scene, string name) =>
        scene.ContinuityAnnotations.Any(annotation => annotation.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
