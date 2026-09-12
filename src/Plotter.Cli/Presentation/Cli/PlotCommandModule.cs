using System.Globalization;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class PlotCommandModule
{
    public static int RunAdd(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel plot add <id> [--description D] [--start T] [--end T]");
        PlotCommands.AddPlot(
            workspace,
            id,
            FindOptionValue(args, "--description"),
            ParseTime(FindOptionValue(args, "--start")),
            ParseTime(FindOptionValue(args, "--end")));
        return 0;
    }

    public static int RunList(NovelWorkspace workspace)
    {
        foreach (var plot in workspace.Plots.Values.OrderBy(plot => plot.Id.Value, StringComparer.OrdinalIgnoreCase))
            Console.WriteLine($"{plot.Id.Value,-12} {plot.Description ?? string.Empty}");
        return 0;
    }

    public static int RunRemove(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var id = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel plot remove <id>");
        PlotCommands.RemovePlot(workspace, id);
        return 0;
    }

    private static StoryTime? ParseTime(string? value) =>
        value is null ? null : new StoryTime(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));

    private static string? FindOptionValue(IReadOnlyList<string> args, string option)
    {
        for (var index = 0; index < args.Count; index++)
            if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Count)
                return args[index + 1];
        return null;
    }
}
