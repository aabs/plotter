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
- Q: What information must each scene be able to capture? → A: Narrative position, date-time, duration, location, participants, chapter, plot thread, POV character, status, and notes, with related entities represented by their IDs.
- Q: How should the manuscript-order view present scenes? → A: Support `novel scenes list --order manuscript`, group scenes by Act, label each scene with its chapter, Scene ID, title, and story date-time, and mark backward story-time transitions with `[FLASHBACK]`.
- Q: How should character continuity views work? → A: Provide a chronological itinerary for one character and a date-filtered lane matrix for multiple characters, with filters for participant groups, characters, and locations to expose continuity problems.
- Q: How should location continuity views work? → A: Provide location occupancy timelines, density summaries, and point-in-time location queries, using explicit scene durations when available and inferred occupancy until the next known participant scene when a duration is absent.
- Q: How should plot-thread continuity be represented? → A: Provide a scene-by-thread matrix using primary, secondary, absent, and not-classified symbols, plus a per-thread timeline with optional scene annotations such as setup, first clue, false lead, recovered, and revealed.
- Q: How should scene inspection, continuity audits, and travel analysis work? → A: Provide a structured scene detail card, classified gap and audit reports that distinguish errors from warnings and information, and an optional travel view using explicit or modeled travel times when available.
- Q: How should users interact with the tool across terminals, scripts, and visual artifacts? → A: Provide composable stable-output commands, a TUI over the same canonical queries, and derived exports in common machine-readable, documentation, calendar, graph, and diagram formats.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Record scene context (Priority: P1)

As a novelist, I want to create scenes and assign their narrative position, temporal details, chapter, plot thread, participants, location, POV character, status, and notes so that every scene has a clear place in the story and manuscript.

**Why this priority**: Attributing scene context is the core value of the tool and is required before any timeline or interaction analysis is useful.

**Independent Test**: Create a novel workspace, add a scene, assign its supported metadata, save it, and retrieve it with the same values intact.

**Initialization workflow**:

```text
mkdir my-novel
cd my-novel
novel init
```

The default workflow creates or opens the canonical `novel.toml` workspace file in the current folder. Multiple novel files may be selected explicitly:

```text
novel init --file ~/novels/my-novel.toml
```

The initial scene/entity workflow is:

```text
novel participant add Mara
novel location add "Boarding house"
novel scene add S001
novel scene set S001 --date-time 1928-06-14T08:10 --participant Mara --location "Boarding house"
novel scene list
novel scene show S001
```

**Acceptance Scenarios**:

1. **Given** an empty current folder, **When** the writer runs `novel init`, **Then** the tool creates a canonical TOML workspace in that folder and reports the selected file.
2. **Given** an explicit path, **When** the writer runs `novel init --file ~/novels/my-novel.toml`, **Then** the tool initializes or opens that workspace without changing the current folder's workspace.
3. **Given** an initialized novel workspace, **When** the writer adds a scene with a non-empty Scene ID, **Then** the scene is stored under that identifier and can be listed.
4. **Given** an existing scene and stored participant and location records, **When** the writer assigns participant IDs and a location ID, **Then** the scene references those records by ID and displays their current properties.
5. **Given** a scene with narrative position, date-time, duration, chapter, plot, POV character, status, and notes, **When** the writer views it, **Then** each assigned value is shown with its related entity names resolved from IDs.
6. **Given** a scene with only some context, **When** the writer views it, **Then** the assigned values are shown and missing values are clearly identified rather than silently invented.
7. **Given** an existing scene, **When** the writer changes or removes an attribution, **Then** the updated scene no longer reports the previous value as current.

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
5. **Given** scenes with Acts, chapters, narrative positions, and story date-times, **When** the writer runs `novel scenes list --order manuscript`, **Then** scenes are grouped by Act and listed in manuscript order with chapter, Scene ID, title, and story date-time.
6. **Given** a scene whose story date-time precedes the preceding dated scene in manuscript order, **When** the writer requests the manuscript-order view, **Then** that scene is marked `[FLASHBACK]`.

Example human-readable output:

```text
Act 1
  Ch 01  S001  The envelope arrives             1928-06-14 08:10
  Ch 01  S002  A name from the past             1919-11-03 14:00  [FLASHBACK]
  Ch 02  S003  The railway station              1928-06-14 11:30

Act 2
  Ch 12  S034  Ballroom confrontation           1928-06-14 21:30
```

7. **Given** scenes matching `1928-06-14` through `1928-06-15`, **When** the writer runs `novel scenes timeline --from 1928-06-14 --to 1928-06-15`, **Then** the tool groups matching scenes under date headings and displays each scene in chronological order with its time, Scene ID, title, location, participants, and available duration, plot-thread, and notes information.

Example human-readable output:

```text
1928-06-14
08:10  S031  Mara wakes in the boarding house
       Boarding house · Mara
       The telegram has arrived.

11:30  S032  Meeting at the railway station
       Flinders Street Station · Mara, Elias
       Thread: The missing ledger

14:00  S033  The train is delayed
       Station café · Mara, Elias, Porter
       Duration: 35m

21:30  S034  Ballroom confrontation
       Hotel ballroom · Mara, Elias, Inspector Vale
       Duration: 45m
```

---

### User Story 3 - Analyze character continuity (Priority: P1)

As a novelist, I want to view a character's chronological itinerary and compare multiple characters in parallel lanes so that I can find impossible travel, unexplained disappearances, accidental co-presence, and missing scenes between encounters.

**Why this priority**: Character continuity is a high-value use of the scene timeline and can reveal contradictions that are difficult to find by reviewing scenes individually.

**Independent Test**: Record dated scenes involving one or more characters, request an itinerary and a multi-character lane view, and verify that each scene appears in the correct character position and time row.

**Acceptance Scenarios**:

1. **Given** a participant with dated scenes, **When** the writer runs `novel character show Mara --timeline`, **Then** the tool displays a chronological itinerary titled `Mara — chronological itinerary` with each scene's time, location, Scene ID, and title or summary.
2. **Given** multiple participants with scenes on the same date, **When** the writer runs `novel scenes lanes --characters Mara,Elias,Vale --date 1928-06-14`, **Then** the tool displays a time-row matrix with one lane per selected participant and scene/location details in each applicable cell.
3. **Given** a lane view where a participant is not present in a scene at a particular time, **When** the view is rendered, **Then** that lane shows an explicit empty marker rather than implying presence.
4. **Given** a large cast, **When** the writer filters with `novel lanes --group "The conspiracy"`, repeated `--character` options, or `--location London`, **Then** only matching participant lanes and scene rows are included.
5. **Given** characters whose dated scenes suggest overlapping locations, long travel, unexplained absence, or missing encounters, **When** the writer reviews the itinerary or lane matrix, **Then** the relevant times, locations, participants, and scene IDs are visible together for continuity investigation.

Example character itinerary:

```text
Mara — chronological itinerary

08:10  Boarding house       S031  Wakes and reads telegram
11:30  Railway station      S032  Meets Elias
14:00  Station café         S033  Waits for train
21:30  Hotel ballroom       S034  Confronts Inspector Vale
```

Example character lanes:

```text
Time   Mara                  Elias                Vale
-----  --------------------  -------------------  --------------------
08:10  S031 Boarding house   —                    —
11:30  S032 Station           S032 Station         —
14:00  S033 Café              S033 Café            —
21:30  S034 Ballroom          S034 Ballroom        S034 Ballroom
```

---

### User Story 4 - Analyze location occupancy (Priority: P1)

As a novelist, I want to see when scenes occupy a location and which characters are present at a given story-world time so that I can maintain setting continuity and plan recurring locations.

**Why this priority**: Locations behave like physical stages, and occupancy views expose gaps, overlaps, and unexplained appearances that are difficult to find from scene records alone.

**Independent Test**: Record scenes at a location with durations and participants, request the location timeline and density summary, and query who is present at a specific time.

**Acceptance Scenarios**:

1. **Given** scenes assigned to a location, **When** the writer runs `novel location show "Hotel ballroom" --timeline`, **Then** the tool lists occupied time ranges in chronological order with Scene IDs and titles or summaries.
2. **Given** locations with scenes across a workspace, **When** the writer requests a location occupancy summary, **Then** the tool displays each location's scene count, first use date, and last use date.
3. **Given** a story-world timestamp, **When** the writer runs `novel where --at 1928-06-14T15:00`, **Then** the tool reports each participant's active location or explicitly reports that no location is recorded.
4. **Given** a participant and story-world timestamp, **When** the writer runs `novel where Mara --at 1928-06-14T15:00`, **Then** the tool reports Mara's active location or that no location is recorded.
5. **Given** a scene without an explicit duration, **When** a point-in-time location query occurs after its start and before that participant's next known scene, **Then** the participant is treated as occupying that scene's location.

Example location timeline:

```text
Hotel ballroom

21:30–22:15  S034  Ballroom confrontation
22:15–22:40  S035  Vale searches the balcony
23:00–23:20  S036  Elias returns alone
```

Example location occupancy summary:

```text
Location              Scenes   First use       Last use
--------------------  ------   ---------------  ----------------
Hotel ballroom        4        1928-06-14       1928-06-15
Railway station       3        1928-06-14       1928-06-16
Boarding house        8        1928-06-10       1928-06-20
```

Example active-location output:

```text
Mara is at Station café.
Elias is at Station café.
Inspector Vale has no recorded location.
```

---

### User Story 5 - Analyze plot-thread continuity (Priority: P1)

As a novelist, I want to compare scenes against plot threads and inspect each thread's progression so that I can find dormant, overloaded, unclassified, or unresolved subplots.

**Why this priority**: A plot-thread matrix provides a compact subplot spreadsheet that reveals dramatic function and continuity patterns across the manuscript.

**Independent Test**: Assign plot threads and classifications to scenes, request the matrix and a single-thread timeline, and verify that classifications and annotations appear against the correct scenes.

**Acceptance Scenarios**:

1. **Given** scenes associated with plot threads, **When** the writer runs `novel threads matrix`, **Then** the tool displays a grid of scenes against plot-thread columns in manuscript order.
2. **Given** a scene-thread relationship classified as primary, secondary, absent, or not classified, **When** the matrix is rendered, **Then** the cell displays `●`, `○`, `·`, or `?` respectively.
3. **Given** a plot thread with scene annotations, **When** the writer runs `novel thread show "Missing ledger"`, **Then** the tool lists matching Scene IDs and annotations in narrative order.
4. **Given** plot threads that disappear for too long, chapters carrying many threads, scenes with no classified thread, or threads without a resolving scene, **When** the writer reviews the matrix or thread timeline, **Then** the relevant scene coverage and annotations are visible for continuity review.

Example plot-thread matrix:

```text
Scene  Missing ledger  Political plot  Mara/Elias  Personal grief
-----  --------------  ---------------  ----------  --------------
S031   setup           ·                ●           ●
S032   ●               ·                ●           ·
S033   ●               ●                ●           ·
S034   ●               ●                ●           ·
S035   ·               ●                ·           ●
```

Example per-thread timeline:

```text
S002  Mentioned
S011  First clue
S019  False lead
S032  Recovered
S034  Revealed
```

---

### User Story 6 - Inspect a scene detail card (Priority: P1)

As a novelist, I want a structured scene card that links scene metadata with adjacent scenes and continuity findings so that it is the primary destination for inspecting or editing one scene.

**Why this priority**: Most other views identify a scene; the detail card provides the complete context needed to understand or correct it.

**Independent Test**: Open a scene by ID and verify that its metadata, relationships, previous/next scenes, elapsed time, flags, and summary are shown together.

**Acceptance Scenarios**:

1. **Given** an existing scene, **When** the writer runs `novel scene show S034`, **Then** the tool displays a structured card containing manuscript position, story time, location, POV, participants, plot threads, continuity context, flags, and summary.
2. **Given** a scene with adjacent scenes, **When** the detail card is shown, **Then** it identifies the previous and next scenes and the elapsed time since the previous scene when calculable.
3. **Given** a scene with continuity findings, **When** the detail card is shown, **Then** findings are displayed as flags without changing the underlying scene data.
4. **Given** an interactive terminal interface, **When** the writer presses Enter on a scene in another view, **Then** the tool opens that scene's detail card.

Example scene card:

```text
S034 · Ballroom confrontation
────────────────────────────────
Manuscript: Chapter 12, position 42
Story time: 1928-06-14 21:30–22:15
Location:   Hotel ballroom
POV:        Mara
Participants:
  - Mara
  - Elias
  - Inspector Vale

Plot threads:
  - Missing ledger: primary
  - Political conspiracy: secondary

Continuity:
  Previous scene: S033, Station café, 14:00
  Next scene:     S035, Balcony, 22:15
  Elapsed time since previous: 7h 30m

Flags:
  ! Elias has no recorded travel from the station
  ! Inspector Vale's previous location is unknown

Summary:
  Mara confronts Vale while Elias attempts to retrieve the ledger.
```

---

### User Story 7 - Review gaps and audit findings (Priority: P1)

As a novelist, I want reports of time gaps, travel concerns, participant continuity, and location continuity so that I can focus review on likely problems without treating every omission as an error.

**Why this priority**: Gaps and audits make hidden continuity risks actionable while respecting intentional uncertainty in fiction.

**Independent Test**: Create scenes with known gaps, overlaps, missing times, and explicit uncertainty annotations, run individual and combined audits, and verify severity and annotations.

**Acceptance Scenarios**:

1. **Given** consecutive scenes with known times and locations, **When** the writer runs `novel continuity gaps`, **Then** the report shows each transition, source and destination, gap duration, and classification.
2. **Given** continuity findings, **When** the writer runs `novel audit time`, `novel audit travel`, `novel audit participants`, `novel audit locations`, or `novel audit all`, **Then** the selected audit report is produced without treating unrelated findings as errors.
3. **Given** a hard conflict, a possible concern, or a non-problematic omission, **Then** the report labels it `ERROR`, `WARN`, or `INFO` respectively.
4. **Given** explicit annotations such as `time-confidence: approximate`, `travel-status: implied`, or `continuity-status: intentional`, **When** audits run, **Then** those annotations are shown and suppress or qualify inappropriate findings.

Example gap report:

```text
Between     From                         To                         Gap
----------  ---------------------------  -------------------------  --------
S031→S032   Boarding house, 08:10        Railway station, 11:30     3h 20m
S032→S033   Railway station, 11:30       Station café, 14:00        2h 30m
S033→S034   Station café, 14:35           Ballroom, 21:30             6h 55m
```

Gap classes are `Normal` for 0–6 hours, `Long` for 6–24 hours, `Overnight` for 24 hours or more, and `Unknown` when timestamp or duration data is missing.

Example audit findings:

```text
ERROR  S041 and S042 overlap, but Mara appears in both locations.
WARN   S052 has a date but no time.
INFO   18 scenes have no duration.
```

---

### User Story 8 - Analyze travel and causality (Priority: P2)

As a novelist, I want to review the time available for a character to move between locations and optionally compare it with a travel-time model so that geography supports believable causality.

**Why this priority**: Travel analysis is especially valuable for constrained geography, investigations, military operations, and simultaneous plot lines, but requires optional location data.

**Independent Test**: Record a participant's consecutive locations and query travel availability, then provide travel estimates and verify that modeled routes are reported.

**Acceptance Scenarios**:

1. **Given** a participant with consecutive dated scenes, **When** the writer runs `novel travel Mara --date 1928-06-14`, **Then** the report lists each location and the time available before the next scene.
2. **Given** locations with a travel-time model, **When** the writer requests a richer travel report, **Then** the report lists feasible destination routes and estimated travel times.
3. **Given** no coordinates or travel-time model, **When** the writer requests travel analysis, **Then** the report still shows available time but does not invent route durations.

Example travel report:

```text
11:30  Railway station
       ↓ 2h 30m available
14:00  Station café
       ↓ 6h 55m available
21:30  Hotel ballroom
```

---

### User Story 9 - Track character interactions (Priority: P2)

As a novelist, I want to record and inspect interactions between participants within scenes so that I can follow relationships and ensure important encounters occur in the intended sequence.

**Why this priority**: Interaction tracking extends scene attribution into relationship and continuity planning, but the tool remains useful without it.

**Independent Test**: Add an interaction to a scene involving two participants, retrieve interaction history for either participant, and verify that the scene and counterpart are reported.

**Acceptance Scenarios**:

1. **Given** a scene with at least two participants, **When** the writer records an interaction with a type or description, **Then** the interaction is associated with that scene and its participants.
2. **Given** a participant with recorded interactions, **When** the writer requests that participant's interaction history, **Then** interactions are listed with their scenes, dates, counterpart participants, and descriptions.
3. **Given** an interaction that was recorded incorrectly, **When** the writer edits or removes it, **Then** future interaction history reflects the correction.
4. **Given** an interaction references a participant not present in its scene, **When** the writer attempts to save it, **Then** the tool rejects the inconsistency with an actionable message.

---

### User Story 10 - Organize scenes by plot (Priority: P2)

As a novelist, I want to define plots and associate scenes with them so that I can track the scenes that contribute to each narrative thread and its time span.

**Why this priority**: Plot-level organization provides a higher-level view of the novel while preserving scene-level detail.

**Independent Test**: Create a plot with optional start and end times, associate scenes with it, and retrieve the plot with its related scenes.

**Acceptance Scenarios**:

1. **Given** an initialized novel workspace, **When** the writer creates a plot with a non-empty Plot ID, **Then** the plot is stored and can be listed.
2. **Given** an existing plot and scenes, **When** the writer annotates scenes with the Plot ID, **Then** the plot reports those related scenes.
3. **Given** a plot with a start time, end time, both, or neither, **When** the writer views the plot, **Then** the available time bounds and missing bounds are shown explicitly.
4. **Given** a plot with an end time earlier than its start time, **When** the writer attempts to save it, **Then** the tool rejects it with an actionable error.

---

### User Story 11 - Share and recover novel data (Priority: P2)

As a novelist, I want my novel metadata to remain in a human-readable project file so that I can back it up, inspect changes, and continue work on another machine.

**Why this priority**: Reliable, portable data is essential for a writing tool and protects the user's planning work.

**Independent Test**: Save a workspace, copy its project data, open the copy, and verify that scenes and interactions are unchanged.

**Acceptance Scenarios**:

1. **Given** a workspace containing scenes and interactions, **When** the writer saves it, **Then** all supported data is persisted in a portable, human-readable representation.
2. **Given** a valid project file, **When** the writer opens it, **Then** the tool restores scenes, attributions, and interactions without requiring external services.
3. **Given** malformed or incompatible project data, **When** the writer attempts to open it, **Then** the tool reports the location and nature of the problem without overwriting the source data.

### User Story 12 - Use composable CLI, TUI, and exports (Priority: P1)

As a novelist or editor, I want consistent commands, interactive browsing, and derived exports over the same scene data so that I can answer continuity questions from scripts, a terminal browser, or external writing tools without maintaining separate data.

**Why this priority**: Consistent projections make the tool useful in automation and editors while preventing the TUI and exports from becoming competing data models.

**Independent Test**: Run a query as plain text, request a machine-readable format, open the same query in the TUI, change views while retaining the selected scene, and export the data to a supported artifact.

**Acceptance Scenarios**:

1. **Given** a valid workspace, **When** the writer runs composable commands such as `novel scene list`, `novel scene show S034`, `novel timeline`, `novel audit`, or `novel export --format csv`, **Then** the commands use stable plain-text output by default.
2. **Given** a query or audit, **When** the writer requests `--format json`, `--format csv`, or `--format sarif` where supported, **Then** the output is machine-readable and contains the same underlying results as the default view.
3. **Given** a valid workspace, **When** the writer opens the TUI, **Then** it provides scene browsing, a detail pane, filters, ordering/view changes, search, date navigation, and access to character lanes and audits over the same query results.
4. **Given** a selected scene in the TUI, **When** the writer changes ordering, filters, or view, **Then** the selected scene remains stable when it is still in the result set and can be opened with Enter.
5. **Given** canonical scene data, **When** the writer exports Markdown, CSV, JSON, iCalendar, Graphviz DOT, Mermaid, HTML, SVG, or terminal text, **Then** the artifact is derived from the canonical data and does not create a separate editable data model.
6. **Given** a new installation, **When** the writer follows the primary workflow, **Then** `novel timeline`, `novel scene show S034`, `novel character timeline Mara`, and `novel audit` provide the initial chronological, detail, character-continuity, and problem-finding views.
7. **Given** an initialized workspace, **When** the writer uses the TUI, **Then** the writer can create, view, update, and remove Scenes, Participants, Locations, Plots, Participant Groups, and Interactions, and can create or remove their supported relationships and annotations through TUI actions backed by the same application services as the CLI.

Example TUI layout:

```text
┌ Scenes: chronological ─────────────────────────────────────────────┐
│ 1928-06-14                                                        │
│ 08:10  S031  Boarding house       Mara                            │
│ 11:30  S032  Railway station      Mara, Elias                     │
│ 14:00  S033  Station café         Mara, Elias, Porter             │
│ 21:30  S034  Hotel ballroom       Mara, Elias, Vale               │
├ Detail ────────────────────────────────────────────────────────────┤
│ S034 Ballroom confrontation                                          │
│ 21:30–22:15 · Chapter 12 · Missing ledger                           │
├ Filters ───────────────────────────────────────────────────────────┤
│ [c]haracter [l]ocation [t]hread [d]ate [o]rder  / search  q quit   │
└────────────────────────────────────────────────────────────────────┘
```

Supported TUI keys include `j`/`k` for scene movement, Enter for details, `/` for search, `f` for filters, `o` for ordering, `v` for view selection, `g` for date navigation, `c` for character lanes, `a` for audits, `e` for editing, and `q` for quitting.

---

### Edge Cases

- A scene may have a date without a time, a time without a date, or explicitly uncertain date/time information.
- A scene may omit any metadata other than its required Scene ID while it is being drafted.
- A scene duration may be unknown, but a supplied duration cannot be negative.
- Dates and times may be displayed using the writer's preferred calendar notation and may require a stated time zone or story-world time zone.
- Multiple scenes may share the same date, time, location, or participants.
- A scene may have no chapter, plot thread, POV character, status, or notes.
- A participant or location may be renamed while retaining its ID; existing scene references must remain associated with the same entity.
- A scene may have no participants, location, or plot while it is being drafted.
- A plot may have no start time, no end time, or no time bounds while it is being drafted.
- A plot's end time may not precede its start time.
- The writer may attempt to create an empty or duplicate Scene ID, Participant ID, Location ID, or Plot ID.
- A Scene ID may resemble `SOME-SCENE-NAME`, but IDs with other non-empty forms must also be accepted.
- A project file may be opened by an older or newer tool version.
- Empty results for a timeline or interaction query must be reported clearly rather than treated as an error.
- A timeline request with identical `--from` and `--to` dates must show scenes from that single date.
- A timeline request with no matching scenes must show the requested range and a clear empty-result message without failing.
- A manuscript-order view may contain scenes without story date-times; those scenes remain in narrative order and are not marked as flashbacks.
- A character itinerary may contain scenes without times; those scenes remain visible with their known date or an explicit unknown-time marker.
- A lane matrix may contain multiple scenes for the same participant and time; all relevant scene IDs must remain visible rather than being collapsed silently.
- A participant absent from a scene must be represented by an empty lane cell, not inferred as present.
- Filters that match no participants or scenes must return a clear empty result.
- A participant group may contain no participants or may overlap with other groups.
- A location occupancy timeline may contain gaps between scenes and overlapping scenes at the same location.
- A scene with an explicit duration ends at its start time plus that duration.
- A scene without a duration has inferred occupancy for each participant until that participant's next known scene; no location is inferred for participants not recorded in the scene.
- A point-in-time query may match multiple simultaneous locations for a participant; all matching records must be shown as a continuity conflict.
- A scene may be associated with a plot thread without being classified as primary or secondary; such a cell is shown as not classified.
- A plot-thread matrix may contain no classified threads for a scene, and must retain that scene as a row.
- A plot thread may have long gaps, many concurrent scene associations, or no resolving annotation.
- The first dated scene in manuscript order cannot be marked as a flashback because it has no preceding dated scene for comparison.
- A manuscript-order view may contain multiple Acts and chapters, including empty or unnamed groups only when represented by stored narrative-position data.
- A scene may have no previous or next scene, or elapsed time may be unknown.
- Gap reports may classify transitions as Normal, Long, Overnight, or Unknown.
- Audits must preserve intentional uncertainty and must not escalate every missing value to an error.
- A travel report may have available time without a known travel route or duration model.
- A travel model may identify multiple feasible routes or no feasible route between locations.
- A machine-readable export may have no rows; it must remain valid for its format and clearly represent an empty result.
- An export format may not support every human-readable presentation detail; unsupported presentation elements must not change canonical data.
- A selected TUI scene may be filtered out; the interface must clearly indicate that the selection is no longer in the current result set.
- TUI terminal dimensions may be too small for the preferred layout; the tool must retain access to the selected scene and essential controls.
- A command alias and its canonical command must return equivalent underlying results.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The tool MUST allow a writer to initialize and open a novel workspace.
- **FR-001a**: The tool MUST support `novel init`, creating or opening `novel.toml` in the current folder by default, and MUST report the resolved file.
- **FR-001b**: The tool MUST support `novel init --file <path>` for initializing or opening a workspace at an explicit path without changing the current-folder default.
- **FR-001c**: The tool MUST support the initial entity and scene workflow through `novel participant add <name>`, `novel location add <name>`, `novel scene add <scene-id>`, `novel scene set <scene-id> [options]`, `novel scene list`, and `novel scene show <scene-id>`.
- **FR-001d**: `novel scene set` MUST support at least the initial scene attribution options for story date-time, participant ID/name, and location ID/name while preserving the canonical ID references.
- **FR-001e**: Initialization MUST create or open a canonical TOML workspace without overwriting existing data unless the writer explicitly requests replacement.
- **FR-002**: The tool MUST allow a writer to create, view, update, and remove scenes with a non-empty, unique Scene ID and optional title or description.
- **FR-002a**: The tool MUST reject creation or renaming of a scene when the resulting Scene ID is empty or already assigned to another scene.
- **FR-002b**: The tool MUST NOT require Scene IDs to follow a prescribed pattern; values such as `SOME-SCENE-NAME` are valid examples, not a mandatory format.
- **FR-003**: The tool MUST allow a writer to create, view, rename, and remove participants and locations used by scenes, each with a unique, non-empty string ID.
- **FR-003a**: The tool MUST reject creation or renaming of a participant or location when the resulting ID is empty or already assigned to another entity of the same type.
- **FR-003b**: The tool MUST store participant and location records in the TOML project file.
- **FR-003c**: The tool MUST allow participants to be assigned to zero or more named participant groups so continuity views can filter by group.
- **FR-004**: The tool MUST allow a writer to assign the following scene information independently: narrative position, story date-time and certainty, duration, location ID, participant IDs, chapter, Plot IDs, POV Participant ID, status, and notes.
- **FR-004a**: Scene associations MUST reference participants, locations, and plots by their IDs rather than by display names.
- **FR-004b**: The tool MUST allow a writer to set, update, or remove a scene's narrative position, duration, chapter, POV Participant ID, status, and notes.
- **FR-004c**: The tool MUST reject a scene duration that is negative.
- **FR-004d**: The tool MUST reject a POV Participant ID that does not identify a participant assigned to the scene.
- **FR-004e**: The tool MUST allow a writer to create, view, update, and remove plots with a unique, non-empty Plot ID and optional description.
- **FR-004f**: The tool MUST allow a writer to annotate a scene with one or more Plot IDs to indicate its relationship to those plots.
- **FR-004g**: The tool MUST allow a plot to have an optional story-world start time and optional story-world end time.
- **FR-004h**: The tool MUST reject a plot whose end time precedes its start time, with an actionable error message.
- **FR-004i**: The tool MUST reject creation or renaming of a plot when the resulting Plot ID is empty or already assigned to another plot.
- **FR-004j**: The tool MUST allow a scene's association with a Plot ID to be classified as primary, secondary, absent, or not classified.
- **FR-004k**: The tool MUST allow an optional per-scene plot-thread annotation, such as setup, first clue, false lead, recovered, or revealed.
- **FR-004l**: The tool MUST allow explicit continuity annotations including `time-confidence`, `travel-status`, and `continuity-status` with user-provided values.
- **FR-005**: The tool MUST preserve the distinction between an unknown value and a known value that is empty or intentionally removed.
- **FR-006**: The tool MUST allow a writer to update or remove any scene attribution without requiring unrelated attributions to be changed.
- **FR-007**: The tool MUST provide a chronological scene view that supports ordering by date and time and filtering by a date range.
- **FR-007a**: The tool MUST support the command form `novel scenes timeline --from YYYY-MM-DD --to YYYY-MM-DD`, where both date bounds are inclusive.
- **FR-007b**: The human-readable chronological view MUST group scenes under `YYYY-MM-DD` date headings and display each scene as a block in chronological order.
- **FR-007c**: Each scene block MUST display the scene time, Scene ID, title or summary, location, and participants when available, and MUST display available duration, plot-thread, and notes information on additional indented lines.
- **FR-007d**: The chronological view MUST omit unavailable optional fields rather than inventing values or displaying misleading placeholders.
- **FR-007e**: The tool MUST support the command form `novel scenes list --order manuscript`.
- **FR-007f**: The manuscript-order view MUST order scenes by their stored narrative positions, group them under Act headings, and identify each scene's chapter.
- **FR-007g**: Each manuscript-order scene row MUST display the chapter, Scene ID, title or summary, and available story date-time.
- **FR-007h**: The manuscript-order view MUST mark a scene with `[FLASHBACK]` when its known story date-time precedes the preceding dated scene in manuscript order.
- **FR-007i**: The manuscript-order view MUST preserve scenes without story date-times in narrative order and MUST NOT mark them as flashbacks.
- **FR-008**: The chronological view MUST include scenes with incomplete or uncertain dates and visibly identify their temporal status.
- **FR-009**: The tool MUST allow a writer to record, view, update, and remove an interaction associated with a scene and two or more participating characters, including an optional type or description.
- **FR-010**: The tool MUST provide interaction history filtered by participant and include the related scene and available temporal context.
- **FR-010a**: The tool MUST support `novel character show <participant> --timeline` and display the selected participant's dated scenes chronologically with time, location, Scene ID, and title or summary.
- **FR-010b**: The tool MUST support `novel scenes lanes --characters <participant-list> --date YYYY-MM-DD` and display a time-row matrix with one lane per selected participant.
- **FR-010c**: Each lane matrix row MUST display the time and each selected participant's applicable Scene ID and location; an absent participant MUST display an explicit empty marker.
- **FR-010d**: The tool MUST support lane filtering by participant group, repeated participant selection, and location, including the forms `novel lanes --group <group>`, `novel lanes --character <participant>`, and `novel lanes --location <location>`.
- **FR-010e**: Character itinerary and lane views MUST retain all matching scenes when multiple scenes share a participant and time, and MUST report clear empty results when filters match nothing.
- **FR-010f**: The tool MUST support `novel threads matrix` and display a scene-by-plot-thread grid in manuscript order.
- **FR-010g**: The plot-thread matrix MUST use `●` for primary, `○` for secondary, `·` for absent, and `?` for not classified.
- **FR-010h**: The tool MUST support `novel thread show <plot-thread>`, listing matching Scene IDs and their optional plot-thread annotations in manuscript order.
- **FR-010i**: Plot-thread matrix and timeline views MUST preserve scenes without classified threads and threads without resolving annotations.
- **FR-010j**: The tool MUST support `novel scene show <scene-id>` and display a structured detail card containing scene metadata, adjacent scenes, elapsed time when calculable, continuity flags, and summary.
- **FR-010k**: Other scene-listing views MUST provide a path to open the selected scene's detail card; interactive terminal views MUST open it when the writer presses Enter on a scene.
- **FR-011**: The tool MUST validate references and reject interactions whose participants are not assigned to the associated scene, with an actionable error message.
- **FR-011a**: The tool MUST support `novel location show <location> --timeline` and display each matching scene's occupied time range, Scene ID, and title or summary in chronological order.
- **FR-011b**: The tool MUST support a location occupancy summary command, such as `novel locations list --occupancy`, with each location's scene count, first use date, and last use date.
- **FR-011c**: The tool MUST support `novel where --at YYYY-MM-DDTHH:mm` and `novel where <participant> --at YYYY-MM-DDTHH:mm` for point-in-time active-location queries.
- **FR-011d**: A point-in-time location query MUST use an explicit scene duration when present and otherwise infer a participant's occupancy until that participant's next known scene.
- **FR-011e**: A point-in-time query MUST report every matching participant location, explicitly report participants with no recorded location, and identify multiple simultaneous locations as a continuity conflict.
- **FR-011f**: The tool MUST support `novel continuity gaps` and display scene transitions, source and destination context, elapsed gap, and gap class.
- **FR-011g**: Gap classification MUST use `Normal` for 0–6 hours, `Long` for 6–24 hours, `Overnight` for 24 hours or more, and `Unknown` when timestamp or duration data is missing.
- **FR-011h**: The tool MUST support `novel audit time`, `novel audit travel`, `novel audit participants`, `novel audit locations`, and `novel audit all`.
- **FR-011i**: Audit output MUST distinguish `ERROR`, `WARN`, and `INFO` findings, and MUST apply explicit continuity annotations when deciding severity or whether to report a finding.
- **FR-011j**: The tool MUST support `novel travel <participant> --date YYYY-MM-DD` and report consecutive locations with available time between scenes.
- **FR-011k**: When location coordinates or a travel-time model are available, travel reports MUST include feasible routes and estimated travel times; without them, the tool MUST NOT invent route durations.
- **FR-012**: The tool MUST persist work in a portable, human-readable TOML project file that can be backed up and reopened without an external service.
- **FR-012a**: The TOML representation MUST key each scene entry by its unique, non-empty Scene ID.
- **FR-012b**: Each TOML scene entry MUST support scene properties including narrative position, Act, date-time, duration, location ID, participant IDs, chapter, Plot IDs, plot-thread classifications and annotations, continuity annotations, POV Participant ID, status, notes, title or summary, and other supported scene attributes.
- **FR-012c**: The TOML representation MUST store participant and location records with their unique, non-empty string IDs and supported properties.
- **FR-012c1**: The TOML representation MUST store participant group membership so lane views can filter participants by group.
- **FR-012d**: The TOML representation MUST represent scene-to-participant, scene-to-location, and scene-to-plot associations using the corresponding entity IDs.
- **FR-012e**: The TOML representation MUST store plot records with their unique, non-empty Plot IDs, optional start and end times, and supported properties.
- **FR-012f**: The TOML representation MAY store location coordinates and travel-time model data for travel reports.
- **FR-013**: The tool MUST detect malformed, incomplete, or incompatible TOML project data and report actionable diagnostics without destroying the original data.
- **FR-014**: The tool MUST provide clear command-line help, usage examples, and errors for invalid commands, missing values, invalid dates/times, and unknown identifiers.
- **FR-015**: The tool MUST support both human-readable command output and a structured output mode suitable for scripting and inspection.
- **FR-016**: The tool MUST be installable as a globally usable command-line tool from the NuGet package catalog.
- **FR-017**: The tool MUST avoid requiring network access for normal creation, editing, querying, and viewing of an existing local workspace.
- **FR-018**: The tool MUST provide composable commands with stable plain-text output by default, including scene listing/showing, timeline, audit, and export commands.
- **FR-018a**: The tool MUST use a consistent noun-based command vocabulary including `scene`, `timeline`, `calendar`, `character`, `location`, `thread`, `audit`, and `export`.
- **FR-018b**: The tool MUST support equivalent projections through command forms including `novel scene list`, `novel scene show <id>`, `novel timeline`, `novel audit`, `novel export`, `novel character`, `novel location`, and `novel thread`.
- **FR-019**: The tool MUST support explicit machine-readable output formats including JSON, CSV, and SARIF for applicable query and audit commands.
- **FR-019a**: The tool MUST support derived exports including Markdown, CSV, JSON, iCalendar, Graphviz DOT, Mermaid, HTML, SVG, and terminal-friendly text reports.
- **FR-019b**: Graph exports MUST support participant and location relationships through forms such as `novel export graph --by participants` and `novel export graph --by locations`.
- **FR-019c**: All exports MUST be derived from canonical scene data and MUST NOT create a separate editable data model.
- **FR-020**: The tool MUST provide a TUI that browses the same query results as the composable commands and does not maintain a separate scene data model.
- **FR-020a**: The TUI MUST provide scene browsing, detail display, filtering by character/location/thread/date, ordering changes, view changes, search, date navigation, character lanes, audit access, and scene editing.
- **FR-020b**: The TUI MUST support `j`/`k`, Enter, `/`, `f`, `o`, `v`, `g`, `c`, `a`, `e`, and `q` according to the documented interactions.
- **FR-020c**: The TUI MUST preserve the selected Scene ID across view, order, and filter changes whenever that scene remains in the result set.
- **FR-020d**: The TUI MUST provide create, view, update, and remove actions for Scenes, Participants, Locations, Plots, Participant Groups, and Interactions.
- **FR-020e**: The TUI MUST provide actions for supported relationships and annotations, including scene-to-participant, scene-to-location, scene-to-plot, POV participant, plot-thread classification/annotation, participant-group membership, and continuity annotations.
- **FR-020f**: TUI create and update actions MUST use the same validation, persistence, and application services as equivalent CLI commands.
- **FR-021**: The tool MUST support calendar projections with `novel calendar --day`, `--week`, and `--month` views.
- **FR-021a**: The initial release MUST prioritize `novel scene list --order manuscript`, `novel timeline --order story-time`, `novel scene show <id>`, `novel character timeline <name>`, `novel location timeline <name>`, `novel audit time`, `novel audit participants`, calendar views, JSON/CSV export, and the TUI over those queries.

### Key Entities

- **Novel Workspace**: A self-contained project representing one novel's planning data and its format/version metadata.
- **Scene**: A distinct narrative unit with a required non-empty unique Scene ID, narrative position, temporal attribution, duration, location, participants, chapter, plot threads, POV character, status, notes, and interactions.
- **Scene ID**: The user-provided non-empty unique key for a scene. It has no mandatory format; `SOME-SCENE-NAME` is an example.
- **Story Date/Time**: The in-world temporal attribution of a scene, including known, unknown, partial, or uncertain values and any relevant time-zone context.
- **Narrative Position**: The writer-assigned position of a scene in the manuscript or intended narrative sequence, including its Act and chapter placement.
- **Act**: A higher-level manuscript grouping used to organize scenes in narrative order.
- **Chapter**: The chapter assignment or label associated with a scene.
- **POV Character**: The participant whose point of view governs a scene, represented by a Participant ID.
- **Scene Status**: The writer-assigned state of a scene, such as a drafting or review state.
- **Scene Notes**: Free-form writer annotations associated with a scene.
- **Participant**: A character or other story entity involved in one or more scenes or interactions, identified by a unique, non-empty string Participant ID and optional participant-group memberships.
- **Participant Group**: A named, reusable grouping of participants used to filter continuity views.
- **Location**: A story-world place associated with one or more scenes, identified by a unique, non-empty string Location ID.
- **Plot**: A narrative thread or higher-level story grouping above scenes, identified by a unique, non-empty Plot ID, with optional story-world start and end times and related scenes.
- **Plot ID**: The user-provided non-empty unique key for a plot.
- **Plot-Thread Classification**: The scene-to-plot relationship value represented as primary, secondary, absent, or not classified.
- **Plot-Thread Annotation**: An optional scene-specific description of a plot thread's dramatic role or progression.
- **Continuity Annotation**: An explicit writer-provided qualification such as time confidence, implied travel, or intentional continuity.
- **Gap Class**: A Normal, Long, Overnight, or Unknown classification for elapsed time between scene transitions.
- **Travel-Time Model**: Optional location data used to estimate feasible routes and travel duration.
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
- **SC-009**: A writer can view and update all requested scene information categories without editing the TOML file manually.
- **SC-010**: A writer can review a manuscript-order listing grouped by Act and chapter, including flashback markers, without manually sorting scenes.
- **SC-011**: A writer can review a participant's chronological itinerary and compare selected participants in date-filtered lanes without manually searching individual scenes.
- **SC-012**: The continuity views make every matching participant/scene occurrence visible, including empty lane cells and repeated same-time occurrences, without silently dropping data.
- **SC-013**: A writer can inspect location occupancy, location reuse dates, and active participant locations at a specified story-world time without manually searching individual scenes.
- **SC-014**: A writer can review plot-thread coverage and per-thread progression, including unclassified scenes and unresolved threads, without manually building a subplot spreadsheet.
- **SC-015**: A writer can inspect a scene's complete detail card, including continuity context and flags, without manually correlating multiple views.
- **SC-016**: A writer can run focused or combined continuity audits and distinguish errors, warnings, and informational findings without treating intentional uncertainty as a hard error.
- **SC-017**: A writer can review available travel time between a participant's locations and, when configured, compare it with modeled route durations.
- **SC-018**: A script or editor integration can consume stable JSON or CSV results from supported queries without parsing human-oriented prose.
- **SC-019**: A writer can move from a scene list to the same scene's detail card in the TUI without losing selection when changing views.
- **SC-020**: At least 95% of supported exported scene records match the corresponding canonical TOML data after round-trip inspection.

## Assumptions

- The initial audience is individual novelists and editors working with local project files; collaboration and concurrent editing are out of scope for the first release.
- A scene's date and time describe story-world chronology, not necessarily the computer's local clock.
- The initial release uses one TOML project file per workspace and provides compatibility diagnostics when the file format changes.
- Scene entries are keyed by their Scene IDs; Scene IDs are user-managed, non-empty, and unique within a workspace, without a required naming convention.
- Participant and location records are stored in the TOML project file with user-managed, non-empty, unique string IDs; scenes reference these entities by ID while display names remain editable properties.
- Participant group memberships are stored in the TOML project file and are used only for filtering continuity views.
- Plot records are stored in the TOML project file with user-managed, non-empty, unique Plot IDs; scenes reference plots by ID.
- Plot-thread classifications and annotations are stored on scene-to-plot associations in the TOML project file.
- Continuity annotations are stored in scene records and are considered by audit reports.
- Location coordinates and travel-time model data are optional; travel analysis remains useful as an available-time report without them.
- Plain-text CLI output is optimized for stable diffs and human inspection; rich visual presentation is provided by the TUI or derived exports.
- JSON, CSV, SARIF, Markdown, iCalendar, Graphviz DOT, Mermaid, HTML, SVG, and text artifacts are projections only; TOML remains the canonical editable data source.
- Plot start and end times use the same story-world temporal context as scene dates and times.
- No external character database or location service is required.
- The tool is expected to run on supported desktop environments where the .NET CLI ecosystem is available.
- Natural-language date parsing, calendar conversion, graphical interfaces, cloud synchronization, and manuscript import are out of scope unless added by a later feature.
- A NuGet package is the distribution mechanism requested by the user; package naming and command naming are implementation decisions for planning.
