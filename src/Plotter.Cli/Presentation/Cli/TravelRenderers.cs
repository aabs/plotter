using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class TravelRenderers
{
  public static void RenderTravel(NovelWorkspace workspace, TravelResult result, TextWriter? writer = null)
  {
    writer ??= Console.Out;
    foreach (var step in result.Steps)
    {
      var time = step.Time?.ToString("HH:mm", CultureInfo.InvariantCulture) ?? "??:??";
      var location = step.LocationId is null ? "" : workspace.Locations.GetValueOrDefault(step.LocationId)?.Name ?? step.LocationId;
      writer.WriteLine($"{time}  {location}");

      if (step.ModeledRoute is { } modeled)
        writer.WriteLine($"       → {FormatDuration(modeled)} route");
      else if (step.AvailableUntilNext is { } available)
        writer.WriteLine($"       ↓ {FormatDuration(available)} available");
    }
  }

  private static string FormatDuration(TimeSpan duration) =>
      duration.TotalHours >= 1
          ? $"{(int)duration.TotalHours}h {duration.Minutes}m"
          : $"{duration.Minutes}m";
}
