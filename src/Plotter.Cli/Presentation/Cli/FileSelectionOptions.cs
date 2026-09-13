namespace Plotter.Cli.Presentation.Cli;

public sealed record FileSelectionOptions(string? ExplicitPath);

public static class FileSelectionParser
{
  public static (FileSelectionOptions Options, string[] RemainingArguments) Parse(IReadOnlyList<string> args)
  {
    var list = args.ToList();
    string? path = null;
    for (var index = 0; index < list.Count; index++)
    {
      if (list[index].Equals("--file", StringComparison.OrdinalIgnoreCase) && index + 1 < list.Count)
      {
        path = list[index + 1];
        list.RemoveRange(index, Math.Min(2, list.Count - index));
        break;
      }
    }
    return (new FileSelectionOptions(path), list.ToArray());
  }
}
