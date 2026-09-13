using System.Globalization;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;

namespace Plotter.Cli.Presentation.Cli;

public static class CharacterCommandModule
{
  public static int RunCharacter(NovelWorkspace workspace, IReadOnlyList<string> args)
  {
    var name = args.ElementAtOrDefault(2) ?? throw new ArgumentException("Usage: novel character timeline <name>");
    CharacterRenderers.RenderItinerary(workspace, CharacterContinuityQueries.GetItinerary(workspace, name));
    return 0;
  }

  public static int RunLanes(NovelWorkspace workspace, IReadOnlyList<string> args)
  {
    var participants = ResolveParticipants(workspace, args);
    var date = ParseDate(FindOptionValue(args, "--date"));
    var location = FindOptionValue(args, "--location");
    CharacterRenderers.RenderLanes(workspace, CharacterContinuityQueries.GetLanes(workspace, participants, date, location));
    return 0;
  }

  private static string[] ResolveParticipants(NovelWorkspace workspace, IReadOnlyList<string> args)
  {
    var participants = new List<string>();

    var characters = FindOptionValue(args, "--characters");
    if (characters is not null)
      participants.AddRange(characters.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

    for (var index = 0; index < args.Count; index++)
      if (args[index].Equals("--character", StringComparison.OrdinalIgnoreCase) && index + 1 < args.Count)
        participants.Add(args[index + 1]);

    var group = FindOptionValue(args, "--group");
    if (group is not null)
      participants.AddRange(CharacterContinuityQueries.ResolveGroup(workspace, group));

    return participants.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
  }

  private static DateTime? ParseDate(string? value) =>
      value is null ? null : DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None);

  private static string? FindOptionValue(IReadOnlyList<string> args, string option)
  {
    for (var index = 0; index < args.Count; index++)
      if (args[index].Equals(option, StringComparison.OrdinalIgnoreCase) && index + 1 < args.Count)
        return args[index + 1];
    return null;
  }
}
