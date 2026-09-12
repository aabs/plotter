using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class CalendarCommandModule
{
    public static int RunCalendar(NovelWorkspace workspace, IReadOnlyList<string> args)
    {
        var period = args.ElementAtOrDefault(1)?.ToLowerInvariant() ?? "month";
        var anchorValue = FindOptionValue(args, "--date") ?? DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var anchor = DateTime.Parse(anchorValue, CultureInfo.InvariantCulture, DateTimeStyles.None).Date;

        var (start, end) = period switch
        {
            "day" => (anchor, anchor.AddDays(1)),
            "week" => (anchor.AddDays(-(int)anchor.DayOfWeek), anchor.AddDays(-(int)anchor.DayOfWeek + 7)),
            _ => (new DateTime(anchor.Year, anchor.Month, 1), new DateTime(anchor.Year, anchor.Month, 1).AddMonths(1)),
        };

        var result = CalendarQueries.GetCalendar(workspace, start, end);
        foreach (var calendarEvent in result.Events)
            Console.WriteLine($"{calendarEvent.StoryDateTime:yyyy-MM-dd HH:mm}  {calendarEvent.SceneId,-8} {calendarEvent.Title}  {calendarEvent.LocationId}");
        return 0;
    }

    private static string? FindOptionValue(IReadOnlyList<string> args, string option)
    {
        for (var index = 0; index < args.Count; index++)
            if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Count)
                return args[index + 1];
        return null;
    }
}
