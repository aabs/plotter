using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public static class SceneValidationService
{
    public static ValidationResult ValidateScene(NovelWorkspace workspace, Scene scene)
    {
        var result = new ValidationResult();

        if (scene.LocationId is { } location && !workspace.Locations.ContainsKey(location.Value))
            result.AddError("SCN-LOC-REF", $"Scene '{scene.Id}' references unknown location '{location.Value}'.", scene.Id.Value);

        foreach (var participantId in scene.ParticipantIds)
            if (!workspace.Participants.ContainsKey(participantId.Value))
                result.AddError("SCN-PART-REF", $"Scene '{scene.Id}' references unknown participant '{participantId.Value}'.", scene.Id.Value);

        foreach (var plot in scene.Plots)
            if (!workspace.Plots.ContainsKey(plot.PlotId.Value))
                result.AddError("SCN-PLOT-REF", $"Scene '{scene.Id}' references unknown plot '{plot.PlotId.Value}'.", scene.Id.Value);

        if (scene.PovParticipantId is { } pov
            && !scene.ParticipantIds.Any(p => p.Value.Equals(pov.Value, StringComparison.OrdinalIgnoreCase)))
            result.AddError("SCN-POV", $"Scene '{scene.Id}' POV participant '{pov.Value}' is not among its participants.", scene.Id.Value);

        return result;
    }
}
