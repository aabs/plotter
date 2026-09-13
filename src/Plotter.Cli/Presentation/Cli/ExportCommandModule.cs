using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class ExportCommandModule
{
  public static int RunExport(NovelWorkspace workspace, IReadOnlyList<string> args)
  {
    var format = FindOptionValue(args, "--format")?.ToLowerInvariant() ?? "text";
    var result = TimelineQueries.GetChronological(workspace, new SceneListQuery());

    var output = format switch
    {
      "json" => Export.BasicExporters.ToJson(result),
      "csv" => Export.BasicExporters.ToCsv(result),
      "markdown" => Export.ArtifactExporters.ToMarkdown(result),
      "ical" => Export.ArtifactExporters.ToICal(workspace, result),
      "html" => Export.ArtifactExporters.ToHtml(result),
      "svg" => Export.ArtifactExporters.ToSvg(result),
      "dot" => Export.ArtifactExporters.ToDot(workspace, "participants"),
      "mermaid" => Export.ArtifactExporters.ToMermaid(workspace, "participants"),
      _ => Export.BasicExporters.ToText(result),
    };

    Console.WriteLine(output);
    return 0;
  }

  public static int RunGraph(NovelWorkspace workspace, IReadOnlyList<string> args)
  {
    var by = FindOptionValue(args, "--by") ?? "participants";
    var format = FindOptionValue(args, "--format")?.ToLowerInvariant() ?? "dot";
    var output = format == "mermaid"
        ? Export.ArtifactExporters.ToMermaid(workspace, by)
        : Export.ArtifactExporters.ToDot(workspace, by);
    Console.WriteLine(output);
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
