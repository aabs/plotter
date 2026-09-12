# Data Model: Novel Scene Timeline

## Storage envelope

A workspace is represented by one TOML file. The file contains a format/version section and collections keyed by stable IDs. The active file is the file selected from the current folder unless an explicit path is supplied.

## Entities

### Novel Workspace

- `format_version`: format compatibility marker.
- `novel_title`: optional display title.
- `scenes`: map keyed by non-empty unique `SceneId`.
- `participants`: map keyed by non-empty unique `ParticipantId`.
- `locations`: map keyed by non-empty unique `LocationId`.
- `plots`: map keyed by non-empty unique `PlotId`.
- `participant_groups`: optional named groups and memberships.

### Scene

- `id`: required, non-empty, unique within workspace; no prescribed pattern.
- `title`/`summary`: optional text.
- `narrative_position`: manuscript ordering value.
- `act`: optional manuscript grouping.
- `chapter`: optional chapter label or number.
- `story_date_time`: optional, with partial/uncertain state.
- `duration`: optional non-negative duration.
- `location_id`: optional reference to a Location.
- `participant_ids`: zero or more Participant references.
- `plot_ids`: zero or more Plot references.
- `plot_thread_classifications`: per-plot value: primary, secondary, absent, or not classified.
- `plot_thread_annotations`: optional per-plot text such as setup or false lead.
- `pov_participant_id`: optional reference that must also occur in `participant_ids`.
- `status`: optional writer-assigned state.
- `notes`: optional free-form text.
- `continuity_annotations`: optional values including `time-confidence`, `travel-status`, and `continuity-status`.
- `interactions`: optional scene-local interaction records.

### Participant

- `id`: required, non-empty, unique Participant ID.
- `name`: editable display name.
- `group_ids`: zero or more Participant Group references.

### Participant Group

- `id`/`name`: stable non-empty identity.
- `participant_ids`: zero or more Participant references.

### Location

- `id`: required, non-empty, unique Location ID.
- `name`: editable display name.
- optional coordinates and travel metadata.

### Plot

- `id`: required, non-empty, unique Plot ID.
- optional description.
- optional story-world `start_time` and `end_time`; end cannot precede start.

### Interaction

- scene-local relationship/encounter record.
- participant references must resolve to participants assigned to the scene.
- optional type and description.

### Continuity and report values

- `GapClass`: Normal (0–6 hours), Long (6–24 hours), Overnight (24+ hours), Unknown.
- `FindingSeverity`: ERROR, WARN, INFO.
- `PlotThreadClassification`: Primary, Secondary, Absent, NotClassified.
- `ContinuityAnnotation`: user-provided qualification; annotations affect audit interpretation, not source facts.

## Relationships and invariants

1. Every Scene ID is non-empty and unique.
2. Every Participant ID, Location ID, and Plot ID is non-empty and unique within its collection.
3. Every referenced ID resolves or produces an actionable validation diagnostic.
4. A Scene's POV participant must be one of its participants.
5. A Scene duration is non-negative.
6. A Plot end time cannot precede its start time.
7. Scene-to-plot classifications and annotations are stored with the relationship, not as global plot state.
8. Removing or renaming display names does not rewrite stable IDs or break associations.
9. Unknown/approximate/implied/intentional values remain distinguishable from absent values.
10. All projections are derived from the same loaded workspace model.

## State and lifecycle

- Workspace: missing → initialized → loaded/edited → persisted.
- Scene and entity records: created → updated → removed; stable IDs remain unique throughout.
- Invalid edits are rejected before replacing the persisted file.
- Export artifacts are derived outputs and never become canonical workspace state.
