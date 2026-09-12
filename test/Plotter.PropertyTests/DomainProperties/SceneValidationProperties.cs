using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.DomainProperties;

public sealed class SceneValidationProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool DuplicateSceneIdIsRejected(SceneId id)
    {
        var workspace = new NovelWorkspace();
        workspace.Scenes[id.Value] = new Scene(id);
        try
        {
            Validation.EnsureUnique(workspace.Scenes, id.Value, "Scene");
            return false;
        }
        catch (InvalidOperationException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool WhitespaceOnlySceneIdIsRejected()
    {
        try
        {
            _ = new SceneId("   ");
            return false;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool UnresolvedLocationReferenceIsInvalid(SceneId sceneId, LocationId missing)
    {
        var workspace = new NovelWorkspace();
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, LocationId: missing);
        return !Validation.ValidateWorkspace(workspace).IsValid;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool UnresolvedParticipantReferenceIsInvalid(SceneId sceneId, ParticipantId missing)
    {
        var workspace = new NovelWorkspace();
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, ParticipantIds: [missing]);
        return !Validation.ValidateWorkspace(workspace).IsValid;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool UnresolvedPlotReferenceIsInvalid(SceneId sceneId, PlotId missing)
    {
        var workspace = new NovelWorkspace();
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, Plots: [new PlotRelationship(missing, PlotThreadClassification.Primary)]);
        return !Validation.ValidateWorkspace(workspace).IsValid;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool NegativeSceneDurationIsRejected(NonNegativeInt minutes)
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
}
