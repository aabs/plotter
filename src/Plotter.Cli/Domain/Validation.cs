namespace Plotter.Cli.Domain;

public sealed record Diagnostic(FindingSeverity Severity, string Code, string Message, string? Location = null);

public sealed class ValidationResult
{
    private readonly List<Diagnostic> _diagnostics = [];

    public IReadOnlyList<Diagnostic> Diagnostics => _diagnostics;

    public bool IsValid => _diagnostics.All(diagnostic => diagnostic.Severity != FindingSeverity.Error);

    public void Add(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);

    public void AddError(string code, string message, string? location = null) => Add(new Diagnostic(FindingSeverity.Error, code, message, location));

    public void AddWarning(string code, string message, string? location = null) => Add(new Diagnostic(FindingSeverity.Warning, code, message, location));
}

public static class Validation
{
    public static string RequiredId(string value, string label)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{label} must be non-empty.", nameof(value));
        return value.Trim();
    }

    public static void EnsureUnique<T>(IReadOnlyDictionary<string, T> values, string id, string label)
    {
        if (values.ContainsKey(id))
            throw new InvalidOperationException($"{label} '{id}' already exists.");
    }

    public static ValidationResult ValidateWorkspace(NovelWorkspace workspace)
    {
        var result = new ValidationResult();

        foreach (var scene in workspace.Scenes.Values)
        {
            if (scene.LocationId is { } location && !workspace.Locations.ContainsKey(location.Value))
                result.AddError("SCN-LOC-REF", $"Scene '{scene.Id}' references unknown location '{location.Value}'.", scene.Id.Value);

            foreach (var participantId in scene.ParticipantIds)
                if (!workspace.Participants.ContainsKey(participantId.Value))
                    result.AddError("SCN-PART-REF", $"Scene '{scene.Id}' references unknown participant '{participantId.Value}'.", scene.Id.Value);

            foreach (var plot in scene.Plots)
                if (!workspace.Plots.ContainsKey(plot.PlotId.Value))
                    result.AddError("SCN-PLOT-REF", $"Scene '{scene.Id}' references unknown plot '{plot.PlotId.Value}'.", scene.Id.Value);

            if (scene.PovParticipantId is { } pov
                && !scene.ParticipantIds.Any(p => p.Value.Equals(pov.Value, StringComparison.OrdinalIgnoreCase)))
                result.AddError("SCN-POV", $"Scene '{scene.Id}' POV participant '{pov.Value}' is not among its participants.", scene.Id.Value);
        }

        foreach (var plot in workspace.Plots.Values)
        {
            if (plot.StartTime is { } start && plot.EndTime is { } end && end.Date < start.Date)
                result.AddError("PLT-BOUNDS", $"Plot '{plot.Id}' end precedes its start.", plot.Id.Value);
        }

        return result;
    }
}
