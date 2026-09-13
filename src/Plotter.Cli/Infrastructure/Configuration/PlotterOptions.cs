namespace Plotter.Cli.Infrastructure.Configuration;

public sealed class PlotterOptions
{
  public string? File { get; set; }

  public string Format { get; set; } = "text";

  public string? TimeZone { get; set; }

  public bool TuiEnabled { get; set; }
}
