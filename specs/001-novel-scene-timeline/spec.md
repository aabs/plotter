# Feature Specification: Novel Scene Timeline

**Feature Branch**: `001-novel-scene-timeline`
**Created**: 2026-09-12
**Status**: Draft
**Input**: User description: "create a .NET CLI tool (published as a tool on nuget) for managing the dates, times and interactions for a novel. The primary objective of this tool is to support the attribution of dates, times, participants and locations onto scenes in a novel."

## Clarifications

### Session 2026-09-12

- Q: What storage format and scene identity rules should the base project data use? → A: Store project data as TOML, key scene entries by a non-empty unique Scene ID, and allow scene properties such as date-time, location, and participants to be stored under each entry; no specific Scene ID format is mandatory.
- Q: How should participants and locations be stored and associated with scenes? → A: Store both in the TOML file with unique, non-empty string IDs, and associate them with scenes by referencing those IDs.
- Q: How should plots relate to scenes and time? → A: Store Plot records above scenes, allow scenes to reference Plot IDs, and support optional story-world start and end times for each Plot.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Record scene context (Priority: P1)

As a novelist, I want to create scenes and assign their date, time, participants, and location so that every scene has a clear place in the story world.

**Why this priority**: Attributing scene context is the core value of the tool and is required before any timeline or interaction analysis is useful.

**Independent Test**: Create a novel workspace, add a scene, assign all four context types, save it, and retrieve it with the same values intact.

**Acceptance Scenarios**:

1. **Given** an initialized novel workspace, **When** the writer adds a scene with a non-empty Scene ID, **Then** the scene is stored under that identifier and can be listed.
2. **Given** an existing scene and stored participant and location records, **When** the writer assigns participant IDs and a location ID, **Then** the scene references those records by ID and displays their current properties.
3. **Given** a scene with only some context, **When** the writer views the scene, **Then** the assigned values are shown and missing values are clearly identified rather than silently invented.
4. **Given** an existing scene, **When** the writer changes or removes an attribution, **Then** the updated scene no longer reports the previous value as current.

---

### User Story 2 - Understand the story timeline (Priority: P1)

As a novelist, I want to view scenes ordered by their in-world dates and times so that I can identify chronology problems while drafting.

**Why this priority**: A usable chronological view turns individual scene metadata into a planning aid and directly supports continuity checking.

**Independent Test**: Add scenes with different dates and times, request a chronological view, and verify that the result is ordered and displays each scene's context.

**Acceptance Scenarios**:

1. **Given** multiple scenes with known dates and times, **When** the writer requests the timeline, **Then** scenes are displayed in chronological order with their identifiers, dates, times, participants, and locations.
2. **Given** scenes with the same date but different times, **When** the writer requests the timeline, **Then** scenes are ordered by time and ties are handled consistently.
3. **Given** scenes with missing or uncertain time information, **When** the writer requests the timeline, **Then** those scenes remain visible and are marked as undated, partially dated, or uncertain instead of being discarded.
4. **Given** a timeline request with a date range, **When** the writer runs it, **Then** only scenes matching the requested range are shown.

---

### User Story 3 - Track character interactions (Priority: P2)

As a novelist, I want to record and inspect interactions between participants within scenes so that I can follow relationships and ensure important encounters occur in the intended sequence.

**Why this priority**: Interaction tracking extends scene attribution into relationship and continuity planning, but the tool remains useful without it.

**Independent Test**: Add an interaction to a scene involving two participants, retrieve interaction history for either participant, and verify that the scene and counterpart are reported.

**Acceptance Scenarios**:

1. **Given** a scene with at least two participants, **When** the writer records an interaction with a type or description, **Then** the interaction is associated with that scene and its participants.
2. **Given** a participant with recorded interactions, **When** the writer requests that participant's interaction history, **Then** interactions are listed with their scenes, dates, counterpart participants, and descriptions.
3. **Given** an interaction that was recorded incorrectly, **When** the writer edits or removes it, **Then** future interaction history reflects the correction.
4. **Given** an interaction references a participant not present in its scene, **When** the writer attempts to save it, **Then** the tool rejects the inconsistency with an actionable message.

---

### User Story 4 - Organize scenes by plot (Priority: P2)

As a novelist, I want to define plots and associate scenes with them so that I can track the scenes that contribute to each narrative thread and its time span.

**Why this priority**: Plot-level organization provides a higher-level view of the novel while preserving scene-level detail.

**Independent Test**: Create a plot with optional start and end times, associate scenes with it, and retrieve the plot with its related scenes.

**Acceptance Scenarios**:

1. **Given** an initialized novel workspace, **When** the writer creates a plot with a non-empty Plot ID, **Then** the plot is stored and can be listed.
2. **Given** an existing plot and scenes, **When** the writer annotates scenes with the Plot ID, **Then** the plot reports those related scenes.
3. **Given** a plot with a start time, end time, both, or neither, **When** the writer views the plot, **Then** the available time bounds and missing bounds are shown explicitly.
4. **Given** a plot with an end time earlier than its start time, **When** the writer attempts to save it, **Then** the tool rejects it with an actionable error.

---

### User Story 5 - Share and recover novel data (Priority: P2)

As a novelist, I want my novel metadata to remain in a human-readable project file so that I can back it up, inspect changes, and continue work on another machine.

**Why this priority**: Reliable, portable data is essential for a writing tool and protects the user's planning work.

**Independent Test**: Save a workspace, copy its project data, open the copy, and verify that scenes and interactions are unchanged.

**Acceptance Scenarios**:

1. **Given** a workspace containing scenes and interactions, **When** the writer saves it, **Then** all supported data is persisted in a portable, human-readable representation.
2. **Given** a valid project file, **When** the writer opens it, **Then** the tool restores scenes, attributions, and interactions without requiring external services.
3. **Given** malformed or incompatible project data, **When** the writer attempts to open it, **Then** the tool reports the location and nature of the problem without overwriting the source data.

### Edge Cases

- A scene may have a date without a time, a time without a date, or explicitly uncertain date/time information.
- Dates and times may be displayed using the writer's preferred calendar notation and may require a stated time zone or story-world time zone.
- Multiple scenes may share the same date, time, location, or participants.
- A participant or location may be renamed while retaining its ID; existing scene references must remain associated with the same entity.
- A scene may have no participants, location, or plot while it is being drafted.
- A plot may have no start time, no end time, or no time bounds while it is being drafted.
- A plot's end time may not precede its start time.
- The writer may attempt to create an empty or duplicate Scene ID, Participant ID, Location ID, or Plot ID.
- A Scene ID may resemble `SOME-SCENE-NAME`, but IDs with other non-empty forms must also be accepted.
- A project file may be opened by an older or newer tool version.
- Empty results for a timeline or interaction query must be reported clearly rather than treated as an error.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The tool MUST allow a writer to initialize and open a novel workspace.
- **FR-002**: The tool MUST allow a writer to create, view, update, and remove scenes with a non-empty, unique Scene ID and optional title or description.
- **FR-002a**: The tool MUST reject creation or renaming of a scene when the resulting Scene ID is empty or already assigned to another scene.
- **FR-002b**: The tool MUST NOT require Scene IDs to follow a prescribed pattern; values such as `SOME-SCENE-NAME` are valid examples, not a mandatory format.
- **FR-003**: The tool MUST allow a writer to create, view, rename, and remove participants and locations used by scenes, each with a unique, non-empty string ID.
- **FR-003a**: The tool MUST reject creation or renaming of a participant or location when the resulting ID is empty or already assigned to another entity of the same type.
- **FR-003b**: The tool MUST store participant and location records in the TOML project file.
- **FR-004**: The tool MUST allow a writer to attribute a scene with a story date, time, date/time certainty, one or more participant IDs, and a location ID, each independently.
- **FR-004a**: Scene associations MUST reference participants and locations by their IDs rather than by display names.
- **FR-004b**: The tool MUST allow a writer to create, view, update, and remove plots with a unique, non-empty Plot ID and optional description.
- **FR-004c**: The tool MUST allow a writer to annotate a scene with one or more Plot IDs to indicate its relationship to those plots.
- **FR-004d**: The tool MUST allow a plot to have an optional story-world start time and optional story-world end time.
- **FR-004e**: The tool MUST reject a plot whose end time precedes its start time, with an actionable error message.
- **FR-004f**: The tool MUST reject creation or renaming of a plot when the resulting Plot ID is empty or already assigned to another plot.
- **FR-005**: The tool MUST preserve the distinction between an unknown value and a known value that is empty or intentionally removed.
- **FR-006**: The tool MUST allow a writer to update or remove any scene attribution without requiring unrelated attributions to be changed.
- **FR-007**: The tool MUST provide a chronological scene view that supports ordering by date and time and filtering by a date range.
- **FR-008**: The chronological view MUST include scenes with incomplete or uncertain dates and visibly identify their temporal status.
- **FR-009**: The tool MUST allow a writer to record, view, update, and remove an interaction associated with a scene and two or more participating characters, including an optional type or description.
- **FR-010**: The tool MUST provide interaction history filtered by participant and include the related scene and available temporal context.
- **FR-011**: The tool MUST validate references and reject interactions whose participants are not assigned to the associated scene, with an actionable error message.
- **FR-012**: The tool MUST persist work in a portable, human-readable TOML project file that can be backed up and reopened without an external service.
- **FR-012a**: The TOML representation MUST key each scene entry by its unique, non-empty Scene ID.
- **FR-012b**: Each TOML scene entry MUST support scene properties including date-time, location ID, participant IDs, Plot IDs, and other supported scene attributes.
- **FR-012c**: The TOML representation MUST store participant and location records with their unique, non-empty string IDs and supported properties.
- **FR-012d**: The TOML representation MUST represent scene-to-participant, scene-to-location, and scene-to-plot associations using the corresponding entity IDs.
- **FR-012e**: The TOML representation MUST store plot records with their unique, non-empty Plot IDs, optional start and end times, and supported properties.
- **FR-013**: The tool MUST detect malformed, incomplete, or incompatible TOML project data and report actionable diagnostics without destroying the original data.
- **FR-014**: The tool MUST provide clear command-line help, usage examples, and errors for invalid commands, missing values, invalid dates/times, and unknown identifiers.
- **FR-015**: The tool MUST support both human-readable command output and a structured output mode suitable for scripting and inspection.
- **FR-016**: The tool MUST be installable as a globally usable command-line tool from the NuGet package catalog.
- **FR-017**: The tool MUST avoid requiring network access for normal creation, editing, querying, and viewing of an existing local workspace.

### Key Entities

- **Novel Workspace**: A self-contained project representing one novel's planning data and its format/version metadata.
- **Scene**: A distinct narrative unit with a required non-empty unique Scene ID, optional title or description, temporal attribution, participants, location, and interactions.
- **Scene ID**: The user-provided non-empty unique key for a scene. It has no mandatory format; `SOME-SCENE-NAME` is an example.
- **Story Date/Time**: The in-world temporal attribution of a scene, including known, unknown, partial, or uncertain values and any relevant time-zone context.
- **Participant**: A character or other story entity involved in one or more scenes or interactions, identified by a unique, non-empty string Participant ID.
- **Location**: A story-world place associated with one or more scenes, identified by a unique, non-empty string Location ID.
- **Plot**: A narrative thread or higher-level story grouping above scenes, identified by a unique, non-empty Plot ID, with optional story-world start and end times and related scenes.
- **Plot ID**: The user-provided non-empty unique key for a plot.
- **Interaction**: A described relationship or encounter between participants within a scene.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: A writer can initialize a workspace and record a scene with date, time, participants, and location in under 3 minutes using documented commands.
- **SC-002**: For a workspace containing 1,000 scenes, a chronological query returns a complete, correctly ordered result in under 2 seconds on a typical personal computer.
- **SC-003**: At least 95% of valid scene attributions entered through documented commands can be retrieved with the same values after closing and reopening the workspace.
- **SC-004**: A writer can identify all scenes involving a selected participant and review their chronological interaction history without manually searching project files.
- **SC-005**: 100% of malformed input and invalid references produce an actionable diagnostic and leave previously saved project data unchanged.
- **SC-006**: A new user can install the published tool, discover the primary workflow from built-in help, and complete the first scene attribution without consulting source code.
- **SC-007**: Writers can copy a project workspace to another machine and continue working without an account, hosted service, or network connection.
- **SC-008**: A writer can identify all scenes associated with a selected plot and review the plot's optional temporal bounds without manually searching project files.

## Assumptions

- The initial audience is individual novelists and editors working with local project files; collaboration and concurrent editing are out of scope for the first release.
- A scene's date and time describe story-world chronology, not necessarily the computer's local clock.
- The initial release uses one TOML project file per workspace and provides compatibility diagnostics when the file format changes.
- Scene entries are keyed by their Scene IDs; Scene IDs are user-managed, non-empty, and unique within a workspace, without a required naming convention.
- Participant and location records are stored in the TOML project file with user-managed, non-empty, unique string IDs; scenes reference these entities by ID while display names remain editable properties.
- Plot records are stored in the TOML project file with user-managed, non-empty, unique Plot IDs; scenes reference plots by ID.
- Plot start and end times use the same story-world temporal context as scene dates and times.
- No external character database or location service is required.
- The tool is expected to run on supported desktop environments where the .NET CLI ecosystem is available.
- Natural-language date parsing, calendar conversion, graphical interfaces, cloud synchronization, and manuscript import are out of scope unless added by a later feature.
- A NuGet package is the distribution mechanism requested by the user; package naming and command naming are implementation decisions for planning.
