using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public sealed record AuditFinding(FindingSeverity Severity, string Code, string Message, IReadOnlyList<string>? SceneIds = null);

public sealed record CommandResult(bool Success, IReadOnlyList<AuditFinding>? Diagnostics = null);

public interface IAuditService
{
    IReadOnlyList<AuditFinding> RunTimeAudit(NovelWorkspace workspace);

    IReadOnlyList<AuditFinding> RunTravelAudit(NovelWorkspace workspace);

    IReadOnlyList<AuditFinding> RunParticipantAudit(NovelWorkspace workspace);

    IReadOnlyList<AuditFinding> RunLocationAudit(NovelWorkspace workspace);

    IReadOnlyList<AuditFinding> RunAll(NovelWorkspace workspace);
}