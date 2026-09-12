using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Auditing;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class AuditCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool GapReportRowsAreOrderedByTime(NovelWorkspace workspace)
    {
        var rows = GapReportService.GetGaps(workspace);
        var positions = workspace.Scenes.ToDictionary(
            pair => pair.Key,
            pair => pair.Value.StoryTime?.Date ?? DateTime.MaxValue,
            StringComparer.OrdinalIgnoreCase);
        for (var index = 1; index < rows.Count; index++)
            if (positions[rows[index].ToSceneId] < positions[rows[index - 1].ToSceneId])
                return false;
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool AuditRendersSeverityPrefixes(NovelWorkspace workspace)
    {
        var findings = new AuditService().RunAll(workspace);
        var writer = new StringWriter();
        AuditRenderers.RenderAudit(findings, writer);
        var output = writer.ToString();
        foreach (var finding in findings)
        {
            var prefix = finding.Severity switch
            {
                FindingSeverity.Error => "ERROR",
                FindingSeverity.Warning => "WARN",
                _ => "INFO",
            };
            if (!output.Contains(prefix, StringComparison.Ordinal))
                return false;
        }
        return true;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool SarifExportIsValidJson(NovelWorkspace workspace)
    {
        var findings = new AuditService().RunAll(workspace);
        var json = SarifExporter.Export(findings);
        try
        {
            using var document = System.Text.Json.JsonDocument.Parse(json);
            return document.RootElement.GetProperty("version").GetString() == "2.1.0";
        }
        catch (System.Text.Json.JsonException)
        {
            return false;
        }
    }
}