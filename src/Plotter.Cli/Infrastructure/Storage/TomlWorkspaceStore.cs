using System.Globalization;
using Plotter.Cli.Domain;
using Plotter.Cli.Serialization;
using Tomlyn;
using Tomlyn.Model;

namespace Plotter.Cli.Infrastructure.Storage;

public interface INovelWorkspaceStore
{
    Task<NovelWorkspace> LoadAsync(string path, CancellationToken cancellationToken = default);

    Task SaveAsync(string path, NovelWorkspace workspace, CancellationToken cancellationToken = default);
}

public sealed class TomlWorkspaceStore : INovelWorkspaceStore
{
    public const string CurrentFormatVersion = "1";

    public async Task<NovelWorkspace> LoadAsync(string path, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(path))
            return new NovelWorkspace();

        var text = await File.ReadAllTextAsync(path, cancellationToken);
        TomlTable model;
        try
        {
            model = Toml.ToModel(text);
        }
        catch (Exception error)
        {
            throw new InvalidDataException($"Malformed TOML in '{path}': {error.Message}", error);
        }

        var dto = ReadDto(model);
        if (dto.FormatVersion is not null && dto.FormatVersion != CurrentFormatVersion)
            throw new InvalidDataException($"Unsupported format version '{dto.FormatVersion}' (expected '{CurrentFormatVersion}').");

        return ToDomain(dto);
    }

    public async Task SaveAsync(string path, NovelWorkspace workspace, CancellationToken cancellationToken = default)
    {
        var model = WriteDto(ToDto(workspace));
        var text = Toml.FromModel(model);
        var temporary = path + ".tmp";
        await File.WriteAllTextAsync(temporary, text, cancellationToken);
        File.Move(temporary, path, true);
    }

    private static TomlWorkspaceDto ToDto(NovelWorkspace workspace)
    {
        var dto = new TomlWorkspaceDto
        {
            FormatVersion = workspace.FormatVersion,
            NovelTitle = workspace.NovelTitle,
        };

        foreach (var participant in workspace.Participants.Values)
            dto.Participants[participant.Id.Value] = new TomlParticipantDto
            {
                Name = participant.Name,
                GroupIds = participant.GroupIds.Count == 0 ? null : participant.GroupIds.Select(g => g.Value).ToList(),
            };

        foreach (var location in workspace.Locations.Values)
            dto.Locations[location.Id.Value] = new TomlLocationDto { Name = location.Name };

        foreach (var plot in workspace.Plots.Values)
            dto.Plots[plot.Id.Value] = new TomlPlotDto
            {
                Description = plot.Description,
                StartTime = FormatTime(plot.StartTime),
                EndTime = FormatTime(plot.EndTime),
            };

        foreach (var group in workspace.ParticipantGroups.Values)
            dto.ParticipantGroups[group.Id.Value] = new TomlParticipantGroupDto
            {
                Name = group.Name,
                ParticipantIds = group.ParticipantIds.Count == 0 ? null : group.ParticipantIds.Select(p => p.Value).ToList(),
            };

        foreach (var scene in workspace.Scenes.Values)
            dto.Scenes[scene.Id.Value] = new TomlSceneDto
            {
                Title = scene.Title,
                NarrativePosition = scene.NarrativePosition,
                Act = scene.Act,
                Chapter = scene.Chapter,
                StoryDateTime = scene.StoryTime?.Date is { } date ? date.ToString("O", CultureInfo.InvariantCulture) : null,
                DurationMinutes = scene.Duration is { } duration ? duration.Value.TotalMinutes : null,
                LocationId = scene.LocationId?.Value,
                ParticipantIds = scene.ParticipantIds.Count == 0 ? null : scene.ParticipantIds.Select(p => p.Value).ToList(),
                Plots = scene.Plots.Count == 0 ? null : scene.Plots.Select(plot => new TomlPlotRelationshipDto
                {
                    PlotId = plot.PlotId.Value,
                    Classification = plot.Classification.ToString().ToLowerInvariant(),
                    Annotation = plot.Annotation,
                }).ToList(),
                PovParticipantId = scene.PovParticipantId?.Value,
                Status = scene.Status,
                Notes = scene.Notes,
                ContinuityAnnotations = scene.ContinuityAnnotations.Count == 0 ? null : scene.ContinuityAnnotations.ToDictionary(a => a.Name, a => a.Value, StringComparer.OrdinalIgnoreCase),
            };

        return dto;
    }

    private static NovelWorkspace ToDomain(TomlWorkspaceDto dto)
    {
        var workspace = new NovelWorkspace
        {
            FormatVersion = dto.FormatVersion ?? CurrentFormatVersion,
            NovelTitle = dto.NovelTitle,
        };

        foreach (var (id, participant) in dto.Participants)
            workspace.Participants[id] = new Participant(new ParticipantId(id), participant.Name ?? id,
                participant.GroupIds?.Select(g => new GroupId(g)).ToArray());

        foreach (var (id, location) in dto.Locations)
            workspace.Locations[id] = new Location(new LocationId(id), location.Name ?? id);

        foreach (var (id, plot) in dto.Plots)
            workspace.Plots[id] = new Plot(new PlotId(id), plot.Description, ParseTime(plot.StartTime), ParseTime(plot.EndTime));

        foreach (var (id, group) in dto.ParticipantGroups)
            workspace.ParticipantGroups[id] = new ParticipantGroup(new GroupId(id), group.Name ?? id,
                group.ParticipantIds?.Select(p => new ParticipantId(p)).ToArray());

        foreach (var (id, scene) in dto.Scenes)
            workspace.Scenes[id] = new Scene(
                new SceneId(id),
                scene.Title,
                ParseTime(scene.StoryDateTime),
                scene.DurationMinutes is { } minutes ? new SceneDuration(TimeSpan.FromMinutes(minutes)) : null,
                scene.LocationId is null ? null : new LocationId(scene.LocationId),
                scene.ParticipantIds?.Select(p => new ParticipantId(p)).ToArray(),
                scene.Plots?.Where(plot => plot.PlotId is not null).Select(plot => new PlotRelationship(
                    new PlotId(plot.PlotId!),
                    Enum.TryParse<PlotThreadClassification>(plot.Classification, ignoreCase: true, out var classification) ? classification : PlotThreadClassification.NotClassified,
                    plot.Annotation)).ToArray(),
                scene.PovParticipantId is null ? null : new ParticipantId(scene.PovParticipantId),
                scene.Status,
                scene.Notes,
                scene.ContinuityAnnotations?.Select(kv => new ContinuityAnnotation(kv.Key, kv.Value)).ToArray(),
                NarrativePosition: scene.NarrativePosition,
                Act: scene.Act,
                Chapter: scene.Chapter);

        return workspace;
    }

    private static TomlTable WriteDto(TomlWorkspaceDto dto)
    {
        var root = new TomlTable();
        root["format_version"] = dto.FormatVersion ?? CurrentFormatVersion;
        if (dto.NovelTitle is not null)
            root["novel_title"] = dto.NovelTitle;

        var participants = new TomlTable();
        foreach (var (id, participant) in dto.Participants)
        {
            var table = new TomlTable { ["name"] = participant.Name ?? id };
            if (participant.GroupIds is { Count: > 0 })
                table["group_ids"] = participant.GroupIds.ToArray();
            participants[id] = table;
        }
        if (participants.Count > 0)
            root["participants"] = participants;

        var locations = new TomlTable();
        foreach (var (id, location) in dto.Locations)
            locations[id] = new TomlTable { ["name"] = location.Name ?? id };
        if (locations.Count > 0)
            root["locations"] = locations;

        var plots = new TomlTable();
        foreach (var (id, plot) in dto.Plots)
        {
            var table = new TomlTable();
            if (plot.Description is not null)
                table["description"] = plot.Description;
            if (plot.StartTime is not null)
                table["start_time"] = plot.StartTime;
            if (plot.EndTime is not null)
                table["end_time"] = plot.EndTime;
            plots[id] = table;
        }
        if (plots.Count > 0)
            root["plots"] = plots;

        var groups = new TomlTable();
        foreach (var (id, group) in dto.ParticipantGroups)
        {
            var table = new TomlTable { ["name"] = group.Name ?? id };
            if (group.ParticipantIds is { Count: > 0 })
                table["participant_ids"] = group.ParticipantIds.ToArray();
            groups[id] = table;
        }
        if (groups.Count > 0)
            root["participant_groups"] = groups;

        var scenes = new TomlTable();
        foreach (var (id, scene) in dto.Scenes)
        {
            var table = new TomlTable();
            if (scene.Title is not null)
                table["title"] = scene.Title;
            if (scene.NarrativePosition is { } narrativePosition)
                table["narrative_position"] = narrativePosition;
            if (scene.Act is not null)
                table["act"] = scene.Act;
            if (scene.Chapter is not null)
                table["chapter"] = scene.Chapter;
            if (scene.StoryDateTime is not null)
                table["story_date_time"] = scene.StoryDateTime;
            if (scene.DurationMinutes is { } minutes)
                table["duration_minutes"] = minutes;
            if (scene.LocationId is not null)
                table["location_id"] = scene.LocationId;
            if (scene.ParticipantIds is { Count: > 0 })
                table["participant_ids"] = scene.ParticipantIds.ToArray();
            if (scene.Plots is { Count: > 0 })
            {
                var scenePlots = new TomlTable();
                foreach (var plot in scene.Plots)
                {
                    var plotTable = new TomlTable { ["classification"] = plot.Classification ?? "not_classified" };
                    if (plot.Annotation is not null)
                        plotTable["annotation"] = plot.Annotation;
                    scenePlots[plot.PlotId!] = plotTable;
                }
                table["plots"] = scenePlots;
            }
            if (scene.PovParticipantId is not null)
                table["pov_participant_id"] = scene.PovParticipantId;
            if (scene.Status is not null)
                table["status"] = scene.Status;
            if (scene.Notes is not null)
                table["notes"] = scene.Notes;
            if (scene.ContinuityAnnotations is { Count: > 0 })
            {
                var annotations = new TomlTable();
                foreach (var (name, value) in scene.ContinuityAnnotations)
                    annotations[name] = value;
                table["continuity_annotations"] = annotations;
            }
            scenes[id] = table;
        }
        if (scenes.Count > 0)
            root["scenes"] = scenes;

        return root;
    }

    private static TomlWorkspaceDto ReadDto(TomlTable model)
    {
        var dto = new TomlWorkspaceDto
        {
            FormatVersion = model.TryGetValue("format_version", out var version) ? version as string : null,
            NovelTitle = model.TryGetValue("novel_title", out var title) ? title as string : null,
        };

        if (model.TryGetValue("participants", out var participantsValue) && participantsValue is TomlTable participants)
            foreach (var (id, value) in participants)
                if (value is TomlTable table)
                    dto.Participants[id] = new TomlParticipantDto
                    {
                        Name = table.TryGetValue("name", out var name) ? name as string : null,
                        GroupIds = ReadStringList(table, "group_ids"),
                    };

        if (model.TryGetValue("locations", out var locationsValue) && locationsValue is TomlTable locations)
            foreach (var (id, value) in locations)
                if (value is TomlTable table)
                    dto.Locations[id] = new TomlLocationDto
                    {
                        Name = table.TryGetValue("name", out var name) ? name as string : null,
                    };

        if (model.TryGetValue("plots", out var plotsValue) && plotsValue is TomlTable plots)
            foreach (var (id, value) in plots)
                if (value is TomlTable table)
                    dto.Plots[id] = new TomlPlotDto
                    {
                        Description = table.TryGetValue("description", out var description) ? description as string : null,
                        StartTime = table.TryGetValue("start_time", out var start) ? start as string : null,
                        EndTime = table.TryGetValue("end_time", out var end) ? end as string : null,
                    };

        if (model.TryGetValue("participant_groups", out var groupsValue) && groupsValue is TomlTable groups)
            foreach (var (id, value) in groups)
                if (value is TomlTable table)
                    dto.ParticipantGroups[id] = new TomlParticipantGroupDto
                    {
                        Name = table.TryGetValue("name", out var name) ? name as string : null,
                        ParticipantIds = ReadStringList(table, "participant_ids"),
                    };

        if (model.TryGetValue("scenes", out var scenesValue) && scenesValue is TomlTable scenes)
            foreach (var (id, value) in scenes)
                if (value is TomlTable table)
                    dto.Scenes[id] = new TomlSceneDto
                    {
                        Title = table.TryGetValue("title", out var sceneTitle) ? sceneTitle as string : null,
                        NarrativePosition = table.TryGetValue("narrative_position", out var narrativePosition) && narrativePosition is long position ? (int)position : null,
                        Act = table.TryGetValue("act", out var act) ? act as string : null,
                        Chapter = table.TryGetValue("chapter", out var chapter) ? chapter as string : null,
                        StoryDateTime = table.TryGetValue("story_date_time", out var story) ? story as string : null,
                        DurationMinutes = table.TryGetValue("duration_minutes", out var duration) && duration is double minutes ? minutes : null,
                        LocationId = table.TryGetValue("location_id", out var location) ? location as string : null,
                        ParticipantIds = ReadStringList(table, "participant_ids"),
                        Plots = ReadPlots(table),
                        PovParticipantId = table.TryGetValue("pov_participant_id", out var pov) ? pov as string : null,
                        Status = table.TryGetValue("status", out var status) ? status as string : null,
                        Notes = table.TryGetValue("notes", out var notes) ? notes as string : null,
                        ContinuityAnnotations = table.TryGetValue("continuity_annotations", out var annotations) && annotations is TomlTable annotationTable
                            ? annotationTable.ToDictionary(kv => kv.Key, kv => kv.Value as string ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                            : null,
                    };

        return dto;
    }

    private static List<TomlPlotRelationshipDto>? ReadPlots(TomlTable table)
    {
        if (!table.TryGetValue("plots", out var value) || value is not TomlTable plots)
            return null;
        var result = new List<TomlPlotRelationshipDto>();
        foreach (var (id, plotValue) in plots)
        {
            if (plotValue is not TomlTable plotTable)
                continue;
            result.Add(new TomlPlotRelationshipDto
            {
                PlotId = id,
                Classification = plotTable.TryGetValue("classification", out var classification) ? classification as string : null,
                Annotation = plotTable.TryGetValue("annotation", out var annotation) ? annotation as string : null,
            });
        }
        return result;
    }

    private static List<string>? ReadStringList(TomlTable table, string key)
    {
        if (!table.TryGetValue(key, out var value) || value is not TomlArray array)
            return null;
        return array.OfType<string>().ToList();
    }

    private static string? FormatTime(StoryTime? time) =>
        time?.Date is { } date ? date.ToString("O", CultureInfo.InvariantCulture) : null;

    private static StoryTime? ParseTime(string? value) =>
        value is null ? null : new StoryTime(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
}
