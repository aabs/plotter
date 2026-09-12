using System.Text.Json;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class AuditRenderers
{
    public static void RenderAudit(IReadOnlyList<AuditFinding> findings, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        foreach (var finding in findings)
        {
            var severity = finding.Severity switch
            {
                FindingSeverity.Error => "ERROR",
                FindingSeverity.Warning => "WARN",
                _ => "INFO",
            };
            writer.WriteLine($"{severity}  {finding.Code}  {finding.Message}");
        }
    }

    public static void RenderGaps(IReadOnlyList<Application.Queries.GapRow> rows, TextWriter? writer = null)
    {
        writer ??= Console.Out;
        writer.WriteLine($"{"Between",-12} {"From",-28} {"To",-28} {"Gap",-10} {"Class",-10}");
        writer.WriteLine($"{new string('-', 12)} {new string('-', 28)} {new string('-', 28)} {new string('-', 10)} {new string('-', 10)}");
        foreach (var row in rows)
        {
            var gap = row.Gap is { } value ? FormatGap(value) : "unknown";
            writer.WriteLine($"{row.FromSceneId}→{row.ToSceneId,-9} {row.FromContext,-28} {row.ToContext,-28} {gap,-10} {row.GapClass,-10}");
        }
    }

    private static string FormatGap(TimeSpan gap) =>
        gap.TotalHours >= 1 ? $"{(int)gap.TotalHours}h {gap.Minutes}m" : $"{gap.Minutes}m";
}

public static class SarifExporter
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static string Export(IReadOnlyList<AuditFinding> findings)
    {
        var results = findings.Select(finding => new
        {
            ruleId = finding.Code,
            level = finding.Severity switch
            {
                FindingSeverity.Error => "error",
                FindingSeverity.Warning => "warning",
                _ => "note",
            },
            message = new { text = finding.Message },
            locations = finding.SceneIds is null
                ? Array.Empty<object>()
                : finding.SceneIds.Select(sceneId => new
                {
                    physicalLocation = new
                    {
                        artifactLocation = new { uri = sceneId },
                    },
                }).ToArray(),
        }).ToArray();

        var sarif = new
        {
            version = "2.1.0",
            runs = new[]
            {
                new
                {
                    tool = new
                    {
                        driver = new { name = "Plotter", informationUri = "https://example.invalid/plotter", version = "0.1.0" },
                    },
                    results,
                },
            },
        };

        return JsonSerializer.Serialize(sarif, Options);
    }
}
