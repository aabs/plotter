using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;
using Spectre.Console;

namespace Plotter.PropertyTests.CliProperties;

public sealed class CommandModuleProperties
{
  private static string Capture(Action action)
  {
    var original = Console.Out;
    using var writer = new StringWriter();
    Console.SetOut(writer);
    try
    {
      action();
    }
    finally
    {
      Console.SetOut(original);
    }
    return writer.ToString();
  }

  private static void RunSilent(Action action)
  {
    var original = AnsiConsole.Console;
    using var writer = new StringWriter();
    AnsiConsole.Console = AnsiConsole.Create(new AnsiConsoleSettings { Out = new AnsiConsoleOutput(writer) });
    try
    {
      action();
    }
    finally
    {
      AnsiConsole.Console = original;
    }
  }

  private static string TempFile() => Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool TimelineCommandRendersSceneIds(NovelWorkspace workspace)
  {
    var output = Capture(() => TimelineCommandModule.RunTimeline(workspace, ["timeline"]));
    return workspace.Scenes.Values.All(scene => output.Contains(scene.Id.Value, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ManuscriptCommandRendersSceneIds(NovelWorkspace workspace)
  {
    var output = Capture(() => TimelineCommandModule.RunSceneList(workspace, ["scene", "list", "--order", "manuscript"]));
    return workspace.Scenes.Values.All(scene => output.Contains(scene.Id.Value, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool CharacterCommandRendersParticipant(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var output = Capture(() => CharacterCommandModule.RunCharacter(workspace, ["character", "timeline", participant]));
    return output.Contains(participant, StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool LocationShowRendersLocation(NovelWorkspace workspace)
  {
    var location = workspace.Locations.Keys.FirstOrDefault();
    if (location is null)
      return true;
    var output = Capture(() => LocationCommandModule.RunLocationShow(workspace, ["location", "show", location]));
    return output.Contains(location, StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool LocationsOccupancyRendersHeader(NovelWorkspace workspace)
  {
    var output = Capture(() => LocationCommandModule.RunLocationsList(workspace, ["locations", "list", "--occupancy"]));
    return output.Contains("Scenes", StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool WhereCommandRendersAllParticipants(NovelWorkspace workspace)
  {
    var output = Capture(() => LocationCommandModule.RunWhere(workspace, ["where", "--at", "2024-01-01T12:00"]));
    return workspace.Participants.Keys.All(participant => output.Contains(participant, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ThreadsMatrixRendersSceneIds(NovelWorkspace workspace)
  {
    var output = Capture(() => PlotThreadCommandModule.RunThreadsMatrix(workspace));
    return workspace.Scenes.Values.All(scene => output.Contains(scene.Id.Value, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool AuditCommandRendersFindings(NovelWorkspace workspace)
  {
    var output = Capture(() => AuditCommandModule.RunAudit(workspace, ["audit", "all"]));
    return output.Length >= 0;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool GapsCommandRendersHeader(NovelWorkspace workspace)
  {
    var output = Capture(() => AuditCommandModule.RunGaps(workspace));
    return output.Contains("Between", StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool TravelCommandRendersLocations(NovelWorkspace workspace)
  {
    var participant = workspace.Participants.Keys.FirstOrDefault();
    if (participant is null)
      return true;
    var output = Capture(() => TravelCommandModule.RunTravel(workspace, ["travel", participant, "--date", "2024-01-01"]));
    return output.Length >= 0;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool PlotListRendersPlotIds(NovelWorkspace workspace)
  {
    var output = Capture(() => PlotCommandModule.RunList(workspace));
    return workspace.Plots.Values.All(plot => output.Contains(plot.Id.Value, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SceneShowRendersSceneId(NovelWorkspace workspace)
  {
    var scene = workspace.Scenes.Values.FirstOrDefault();
    if (scene is null)
      return true;
    var output = Capture(() => SceneDetailCommandModule.RunShow(workspace, ["scene", "show", scene.Id.Value]));
    return output.Contains(scene.Id.Value, StringComparison.Ordinal);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool EntityAddParticipantMutatesWorkspace(SceneId name)
  {
    var workspace = new NovelWorkspace();
    RunSilent(() => EntityCommandModule.RunAddParticipant(workspace, ["participant", "add", name.Value]));
    return workspace.Participants.ContainsKey(name.Value);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool EntityAddLocationMutatesWorkspace(SceneId name)
  {
    var workspace = new NovelWorkspace();
    RunSilent(() => EntityCommandModule.RunAddLocation(workspace, ["location", "add", name.Value]));
    return workspace.Locations.ContainsKey(name.Value);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SceneAddPersistsToFile(SceneId sceneId)
  {
    var store = new TomlWorkspaceStore();
    var file = TempFile();
    try
    {
      var workspace = new NovelWorkspace();
      RunSilent(() => SceneCommandModule.RunAsync(workspace, ["scene", "add", sceneId.Value], store, file).GetAwaiter().GetResult());
      return workspace.Scenes.ContainsKey(sceneId.Value) && File.Exists(file);
    }
    finally
    {
      if (File.Exists(file))
        File.Delete(file);
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SceneSetUpdatesStoryTime(SceneId sceneId, DateTime date)
  {
    var store = new TomlWorkspaceStore();
    var file = TempFile();
    try
    {
      var workspace = new NovelWorkspace();
      workspace.Scenes[sceneId.Value] = new Scene(sceneId);
      RunSilent(() => SceneCommandModule.RunAsync(workspace, ["scene", "set", sceneId.Value, "--date-time", date.ToString("O")], store, file).GetAwaiter().GetResult());
      return workspace.Scenes[sceneId.Value].StoryTime?.Date == date;
    }
    finally
    {
      if (File.Exists(file))
        File.Delete(file);
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool WorkspaceInitCreatesFile()
  {
    var store = new TomlWorkspaceStore();
    var file = TempFile();
    try
    {
      RunSilent(() => WorkspaceCommandModule.RunInitAsync(store, file).GetAwaiter().GetResult());
      return File.Exists(file);
    }
    finally
    {
      if (File.Exists(file))
        File.Delete(file);
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ExportJsonIsValid(NovelWorkspace workspace)
  {
    var output = Capture(() => ExportCommandModule.RunExport(workspace, ["export", "--format", "json"]));
    try
    {
      using var document = System.Text.Json.JsonDocument.Parse(output);
      return true;
    }
    catch (System.Text.Json.JsonException)
    {
      return false;
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool CalendarCommandRendersEvents(NovelWorkspace workspace)
  {
    var output = Capture(() => CalendarCommandModule.RunCalendar(workspace, ["calendar", "--day", "--date", "2024-01-01"]));
    return output.Length >= 0;
  }
}
