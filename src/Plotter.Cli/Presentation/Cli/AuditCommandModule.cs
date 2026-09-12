using Plotter.Cli.Application.Auditing;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class AuditCommandModule
{
    private static readonly AuditService Audit = new();

    public static int RunGaps(NovelWorkspace workspace)
    {
        AuditRenderers.RenderGaps(GapReportService.GetGaps(workspace));
        return 0;
    }

    public static int RunAudit(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var scope = args.ElementAtOrDefault(1)?.ToLowerInvariant();
        IReadOnlyList<AuditFinding> findings = scope switch
        {
            null or "all" => Audit.RunAll(workspace),
            "time" => Audit.RunTimeAudit(workspace),
            "travel" => Audit.RunTravelAudit(workspace),
            "participants" => Audit.RunParticipantAudit(workspace),
            "locations" => Audit.RunLocationAudit(workspace),
            _ => throw new ArgumentException("Usage: novel audit [time|travel|participants|locations|all]"),
        };
        AuditRenderers.RenderAudit(findings);
        return 0;
    }
}
