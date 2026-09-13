using Plotter.Cli.Domain;

namespace Plotter.PropertyTests.Oracles;

public static class ReferenceOracles
{
  /// <summary>Canonical projection of a workspace used to compare persisted round trips.</summary>
  public static string CanonicalText(NovelWorkspace workspace)
  {
    var participants = string.Join("|", workspace.Participants.Values.OrderBy(p => p.Id.Value, StringComparer.Ordinal).Select(p => $"{p.Id.Value}={p.Name}"));
    var locations = string.Join("|", workspace.Locations.Values.OrderBy(l => l.Id.Value, StringComparer.Ordinal).Select(l => $"{l.Id.Value}={l.Name}"));
    var plots = string.Join("|", workspace.Plots.Values.OrderBy(p => p.Id.Value, StringComparer.Ordinal).Select(p => $"{p.Id.Value}:{p.Description}"));
    var scenes = string.Join("|", workspace.Scenes.Values.OrderBy(s => s.Id.Value, StringComparer.Ordinal).Select(s =>
        $"{s.Id.Value}:{s.Title}:{s.NarrativePosition}:{s.Act}:{s.Chapter}:{s.StoryTime?.Date:O}:{s.Duration?.Value.TotalMinutes}:{s.LocationId?.Value}:" +
        $"{string.Join(",", s.ParticipantIds.Select(p => p.Value))}:{string.Join(",", s.Plots.Select(p => p.PlotId.Value))}:{s.PovParticipantId?.Value}:{s.Status}:{s.Notes}"));
    return $"{participants}#{locations}#{plots}#{scenes}";
  }
}
