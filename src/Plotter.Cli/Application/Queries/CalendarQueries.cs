using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Queries;

public static class CalendarQueries
{
  public static CalendarResult GetCalendar(NovelWorkspace workspace, DateTime start, DateTime end)
  {
    var events = workspace.Scenes.Values
        .Where(scene => scene.StoryTime?.Date is { } date && date >= start && date < end)
        .OrderBy(scene => scene.StoryTime!.Date!.Value)
        .Select(scene => new CalendarEvent(scene.Id.Value, scene.Title, scene.StoryTime?.Date, scene.LocationId?.Value))
        .ToArray();
    return new CalendarResult(events);
  }
}
