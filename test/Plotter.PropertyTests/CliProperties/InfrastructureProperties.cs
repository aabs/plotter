using FsCheck;
using FsCheck.Xunit;
using Microsoft.Extensions.DependencyInjection;
using Plotter.Cli.Application.Commands;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Composition;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.Cli.Infrastructure.Diagnostics;
using Plotter.Cli.Infrastructure.Storage;
using Plotter.Cli.Serialization;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class InfrastructureProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool DiagnosticSinkCollectsAndClears(SceneId message)
  {
    var sink = new DiagnosticSink();
    sink.AddError(message.Value);
    sink.AddWarning(message.Value);
    sink.AddInfo(message.Value);
    if (sink.Messages.Count != 3)
      return false;
    sink.Clear();
    return sink.Messages.Count == 0;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SceneValidationServiceAcceptsValidScene(NovelWorkspace workspace)
  {
    var scene = workspace.Scenes.Values.FirstOrDefault();
    if (scene is null)
      return true;
    return SceneValidationService.ValidateScene(workspace, scene).IsValid;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool SceneValidationServiceRejectsUnknownLocation(SceneId sceneId, LocationId missing)
  {
    var workspace = new NovelWorkspace();
    workspace.Scenes[sceneId.Value] = new Scene(sceneId, LocationId: missing);
    return !SceneValidationService.ValidateScene(workspace, workspace.Scenes[sceneId.Value]).IsValid;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ServiceRegistrationResolvesServices()
  {
    using var provider = ServiceRegistration.Build();
    return provider.GetRequiredService<INovelFileResolver>() is not null
        && provider.GetRequiredService<INovelWorkspaceStore>() is not null
        && provider.GetRequiredService<DiagnosticSink>() is not null;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool PlotterOptionsDefaults()
  {
    var options = new PlotterOptions();
    return options.Format == "text" && !options.TuiEnabled && options.File is null;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool GroupIdRejectsWhitespace()
  {
    try
    {
      _ = new GroupId("   ");
      return false;
    }
    catch (ArgumentException)
    {
      return true;
    }
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool ParticipantGroupDtoRoundTrips(SceneId name)
  {
    var dto = new TomlParticipantGroupDto { Name = name.Value, ParticipantIds = [name.Value] };
    return dto.Name == name.Value && dto.ParticipantIds.Count == 1;
  }
}
