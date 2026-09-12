using Plotter.Cli.Domain;

namespace Plotter.Cli.Application.Commands;

public static class EntityCommands
{
    public static CommandResult AddParticipant(NovelWorkspace workspace, string name)
    {
        var id = new ParticipantId(Validation.RequiredId(name, "Participant name"));
        Validation.EnsureUnique(workspace.Participants, id.Value, "Participant");
        workspace.Participants[id.Value] = new Participant(id, id.Value);
        return new CommandResult(true);
    }

    public static CommandResult AddLocation(NovelWorkspace workspace, string name)
    {
        var id = new LocationId(Validation.RequiredId(name, "Location name"));
        Validation.EnsureUnique(workspace.Locations, id.Value, "Location");
        workspace.Locations[id.Value] = new Location(id, id.Value);
        return new CommandResult(true);
    }
}