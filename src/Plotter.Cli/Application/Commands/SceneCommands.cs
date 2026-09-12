using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public sealed record SceneUpdate(
    string? Title = null,
    int? NarrativePosition = null,
    string? Act = null,
    string? Chapter = null,
    StoryTime? StoryTime = null,
    SceneDuration? Duration = null,
    LocationId? LocationId = null,
    IReadOnlyList<ParticipantId>? ParticipantIds = null,
    ParticipantId? PovParticipantId = null,
    string? Status = null,
    string? Notes = null);

public static class SceneCommands
{
    public static CommandResult AddScene(NovelWorkspace workspace, string id)
    {
        var sceneId = new SceneId(Validation.RequiredId(id, "Scene ID"));
        Validation.EnsureUnique(workspace.Scenes, sceneId.Value, "Scene");
        workspace.Scenes[sceneId.Value] = new Scene(sceneId, sceneId.Value);
        return new CommandResult(true);
    }

    public static CommandResult SetScene(NovelWorkspace workspace, string id, SceneUpdate update)
    {
        if (!workspace.Scenes.TryGetValue(id, out var scene))
            throw new InvalidOperationException($"Scene '{id}' does not exist.");

        var updated = scene with
        {
            Title = update.Title ?? scene.Title,
            NarrativePosition = update.NarrativePosition ?? scene.NarrativePosition,
            Act = update.Act ?? scene.Act,
            Chapter = update.Chapter ?? scene.Chapter,
            StoryTime = update.StoryTime ?? scene.StoryTime,
            Duration = update.Duration ?? scene.Duration,
            LocationId = update.LocationId ?? scene.LocationId,
            ParticipantIds = update.ParticipantIds ?? scene.ParticipantIds,
            PovParticipantId = update.PovParticipantId ?? scene.PovParticipantId,
            Status = update.Status ?? scene.Status,
            Notes = update.Notes ?? scene.Notes,
        };

        workspace.Scenes[id] = updated;
        return new CommandResult(true);
    }

    public static CommandResult RemoveScene(NovelWorkspace workspace, string id)
    {
        if (!workspace.Scenes.Remove(id))
            throw new InvalidOperationException($"Scene '{id}' does not exist.");
        return new CommandResult(true);
    }
}