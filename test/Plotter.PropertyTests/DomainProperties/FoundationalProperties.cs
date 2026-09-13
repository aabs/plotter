using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.DomainProperties;

public sealed class FoundationalProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool RequiredIdTrimsAndPreserves(SceneId id)
  {
    var value = Validation.RequiredId($"  {id.Value}  ", "ID");
    return value == id.Value && value.Length > 0;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool DistinctSceneIdsRemainDistinct(SceneId first, SceneId second)
  {
    if (first.Value == second.Value)
      return true;
    return !first.Equals(second);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ValidWorkspacePassesValidation(NovelWorkspace workspace) =>
      Validation.ValidateWorkspace(workspace).IsValid;

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool RemovingReferencedParticipantMakesWorkspaceInvalid(NovelWorkspace workspace)
  {
    var scene = workspace.Scenes.Values.FirstOrDefault(candidate => candidate.ParticipantIds.Count > 0);
    if (scene is null)
      return true;
    workspace.Participants.Remove(scene.ParticipantIds[0].Value);
    return !Validation.ValidateWorkspace(workspace).IsValid;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool PovOutsideParticipantsIsInvalid(SceneId sceneId, ParticipantId pov)
  {
    var scene = new Scene(sceneId, PovParticipantId: pov);
    var workspace = new NovelWorkspace();
    workspace.Scenes[scene.Id.Value] = scene;
    return !Validation.ValidateWorkspace(workspace).IsValid;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool NegativeDurationIsRejected(NonNegativeInt minutes)
  {
    try
    {
      _ = new SceneDuration(TimeSpan.FromMinutes(-1 - minutes.Get));
      return false;
    }
    catch (ArgumentException)
    {
      return true;
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool CancelledLoadThrows()
  {
    var store = new TomlWorkspaceStore();
    var path = Path.Combine(Path.GetTempPath(), $"plotter-{Guid.NewGuid():N}.toml");
    try
    {
      File.WriteAllText(path, "format_version = \"1\"\n");
      using var cts = new CancellationTokenSource();
      cts.Cancel();
      try
      {
        store.LoadAsync(path, cts.Token).GetAwaiter().GetResult();
        return false;
      }
      catch (OperationCanceledException)
      {
        return true;
      }
    }
    finally
    {
      if (File.Exists(path))
        File.Delete(path);
    }
  }
}
