namespace Plotter.Cli.Serialization;

public sealed class TomlWorkspaceDto
{
    public string? FormatVersion { get; set; }

    public string? NovelTitle { get; set; }

    public Dictionary<string, TomlParticipantDto> Participants { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, TomlLocationDto> Locations { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, TomlPlotDto> Plots { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, TomlParticipantGroupDto> ParticipantGroups { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, TomlSceneDto> Scenes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

public sealed class TomlParticipantDto
{
    public string? Name { get; set; }

    public List<string>? GroupIds { get; set; }
}

public sealed class TomlLocationDto
{
    public string? Name { get; set; }
}

public sealed class TomlPlotDto
{
    public string? Description { get; set; }

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }
}

public sealed class TomlParticipantGroupDto
{
    public string? Name { get; set; }

    public List<string>? ParticipantIds { get; set; }
}

public sealed class TomlSceneDto
{
    public string? Title { get; set; }

    public int? NarrativePosition { get; set; }

    public string? Act { get; set; }

    public string? Chapter { get; set; }

    public string? StoryDateTime { get; set; }

    public double? DurationMinutes { get; set; }

    public string? LocationId { get; set; }

    public List<string>? ParticipantIds { get; set; }

    public List<TomlPlotRelationshipDto>? Plots { get; set; }

    public string? PovParticipantId { get; set; }

    public string? Status { get; set; }

    public string? Notes { get; set; }

    public Dictionary<string, string>? ContinuityAnnotations { get; set; }
}

public sealed class TomlPlotRelationshipDto
{
    public string? PlotId { get; set; }

    public string? Classification { get; set; }

    public string? Annotation { get; set; }
}
