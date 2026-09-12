using Plotter.Cli.Domain;

namespace Plotter.Cli.Infrastructure.Diagnostics;

public sealed record DiagnosticMessage(FindingSeverity Severity, string Message);

public sealed class DiagnosticSink
{
    private readonly List<DiagnosticMessage> _messages = [];

    public IReadOnlyList<DiagnosticMessage> Messages => _messages;

    public void Add(FindingSeverity severity, string message) => _messages.Add(new DiagnosticMessage(severity, message));

    public void AddError(string message) => Add(FindingSeverity.Error, message);

    public void AddWarning(string message) => Add(FindingSeverity.Warning, message);

    public void AddInfo(string message) => Add(FindingSeverity.Info, message);

    public void Clear() => _messages.Clear();
}