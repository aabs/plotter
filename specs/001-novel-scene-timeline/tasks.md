---
description: "Task list for implementing Novel Scene Timeline"
---

# Tasks: Novel Scene Timeline

**Input**: Design documents from `specs/001-novel-scene-timeline/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md`

**Testing policy**: FsCheck/FsCheck.Xunit property-based tests are mandatory for broad behavior. Every property test must name an invariant and oracle. Example-based tests are permitted only for specific regression defects. Use TDD: write each property first, observe failure, then implement.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish the .NET 10/C# 14 CLI, central package management, analyzers, packaging, and property-test project.

- [x] T001 Create repository-wide `Directory.Packages.props` with central package management and pinned Spectre.Console, Tomlyn, FsCheck, FsCheck.Xunit, xUnit, coverage, and packaging tool versions.
- [x] T002 [P] Configure root `.editorconfig` with nullable/analyzer/style rules required by the constitution.
- [x] T003 Update `src/Plotter.Cli/Plotter.Cli.csproj` for NuGet tool metadata, `net10.0`, C# 14, nullable, warnings-as-errors, analyzers, and central package references.
- [x] T004 [P] Create `test/Plotter.PropertyTests/Plotter.PropertyTests.csproj` targeting `net10.0` with FsCheck, FsCheck.Xunit, xUnit integration, coverage, and a project reference to `src/Plotter.Cli/Plotter.Cli.csproj`.
- [x] T005 [P] Create the planned source directories under `src/Plotter.Cli/Domain`, `Application`, `Infrastructure`, `Presentation`, and `Serialization`.
- [x] T006 [P] Create the property-test directories under `test/Plotter.PropertyTests/Generators`, `Oracles`, `DomainProperties`, `PersistenceProperties`, `ProjectionProperties`, `CliProperties`, and `RegressionTests`.
- [x] T007 Add solution/project membership for `src/Plotter.Cli/Plotter.Cli.csproj` and `test/Plotter.PropertyTests/Plotter.PropertyTests.csproj` in `plotter.slnx`.
- [x] T008 Add CI/build configuration for `dotnet restore`, `dotnet build --no-restore`, `dotnet test`, coverage collection, and `dotnet format --verify-no-changes` in `.github/workflows/ci.yml`.

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Implement the canonical model, file resolution, TOML persistence, query contracts, diagnostics, and projection abstractions required by every story.

**Checkpoint**: No user-story implementation starts until this phase passes its property suite and build gates.

- [x] T009 Define immutable ID/value types and enums in `src/Plotter.Cli/Domain/Identifiers.cs` and `src/Plotter.Cli/Domain/Values.cs`, including non-empty unique IDs, story date/time certainty, durations, gap classes, finding severity, plot-thread classification, and continuity annotations.
- [x] T010 [P] Define immutable domain records for Workspace, Scene, Participant, ParticipantGroup, Location, Plot, Interaction, and relationship metadata in `src/Plotter.Cli/Domain/Entities.cs`.
- [x] T011 [P] Define explicit TOML serialization DTOs separate from domain records in `src/Plotter.Cli/Serialization/TomlDtos.cs`.
- [x] T012 Define domain validation results and actionable diagnostic types in `src/Plotter.Cli/Domain/Validation.cs`, enforcing “POV participant belongs to scene”, non-negative duration, valid Plot time bounds, and reference closure.
- [x] T013 [P] Add FsCheck generators for valid/invalid IDs, dates, durations, scenes, entities, relationships, workspaces, and continuity annotations in `test/Plotter.PropertyTests/Generators/DomainGenerators.cs`.
- [x] T014 [P] Add reusable reference-model oracles for TOML round trips, ordering, classifications, projection equivalence, and audit severity in `test/Plotter.PropertyTests/Oracles/ReferenceOracles.cs`.
- [x] T015 Define `INovelFileResolver` and current-folder/explicit-path resolution in `src/Plotter.Cli/Infrastructure/Configuration/NovelFileResolver.cs`; default to `novel.toml` in the current folder and prevent cross-file leakage.
- [x] T016 Define typed application options for file selection, output format, locale/time settings, and TUI behavior in `src/Plotter.Cli/Infrastructure/Configuration/PlotterOptions.cs`.
- [x] T017 Implement async Tomlyn-backed load/save behind `INovelWorkspaceStore` in `src/Plotter.Cli/Infrastructure/Storage/TomlWorkspaceStore.cs`, accepting and propagating `CancellationToken`, including format/version validation and atomic commit-or-rollback writes.
- [x] T018 [P] Add property tests for ID uniqueness, reference closure, validation invariants, current-folder resolution, explicit file selection, cancellation propagation, and atomic invalid-write behavior in `test/Plotter.PropertyTests/DomainProperties/FoundationalProperties.cs` and `test/Plotter.PropertyTests/PersistenceProperties/FileResolutionProperties.cs`.
- [x] T019 Add the persistence round-trip property `Save(Load(Save(workspace))) == workspace` with shrinking counterexamples in `test/Plotter.PropertyTests/PersistenceProperties/TomlRoundTripProperties.cs`.
- [x] T020 Define application query/result DTOs and async `IProjectionQueryService` contracts with `CancellationToken` in `src/Plotter.Cli/Application/Queries/QueryContracts.cs`.
- [x] T021 Define command/result, audit finding, export, and rendering abstractions in `src/Plotter.Cli/Application/Commands/ApplicationContracts.cs` and `src/Plotter.Cli/Application/Projections/ProjectionContracts.cs`.
- [x] T022 Implement structured logging, expected-result diagnostics, and exception-preserving error boundaries in `src/Plotter.Cli/Infrastructure/Diagnostics/DiagnosticServices.cs`.
- [x] T023 Configure dependency injection and options binding in `src/Plotter.Cli/Program.cs` and `src/Plotter.Cli/Infrastructure/Composition/ServiceRegistration.cs`.
- [x] T024 Add shared projection-equivalence properties for plain text, JSON, CSV, and audit results in `test/Plotter.PropertyTests/ProjectionProperties/ProjectionEquivalenceProperties.cs`.
- [x] T025 Add `test/Plotter.PropertyTests/RegressionTests/RegressionTestPolicy.md` documenting that example tests require a named historical defect and that all other coverage is property-based.

---

## Phase 3: User Story 1 - Record Scene Context (Priority: P1) 🎯 MVP

**Goal**: Create, update, view, and remove scenes and their requested metadata with ID-based references and TOML persistence.

**Independent Test**: Initialize a workspace, create a scene with valid metadata and references, save/reload it, and verify all values and associations remain intact.

### Tests for User Story 1 (write first)

- [x] T026 [P] [US1] Define the scene metadata round-trip invariant for narrative position, Act, chapter, story date/time, duration, location ID, participant IDs, Plot IDs, POV Participant ID, status, notes, and title in `test/Plotter.PropertyTests/DomainProperties/SceneContextProperties.cs`.
- [x] T027 [P] [US1] Define invalid-scene properties for empty/duplicate Scene IDs, negative durations, unresolved IDs, and invalid POV references in `test/Plotter.PropertyTests/DomainProperties/SceneValidationProperties.cs`.
- [x] T028 [P] [US1] Define a regression-only example test only if a concrete prior scene-context defect is discovered in `test/Plotter.PropertyTests/RegressionTests/SceneContextRegressionTests.cs`.

### Implementation for User Story 1

- [x] T029 [US1] Implement workspace initialization/opening plus scene/entity create-update-remove application services in `src/Plotter.Cli/Application/Commands/WorkspaceCommands.cs`, `src/Plotter.Cli/Application/Commands/SceneCommands.cs`, and `src/Plotter.Cli/Application/Commands/EntityCommands.cs`; support `novel init` and `novel init --file <path>`.
- [x] T030 [US1] Implement scene reference and metadata validation in `src/Plotter.Cli/Application/Commands/SceneValidationService.cs`.
- [x] T031 [US1] Implement initialization, scene, participant, and location CLI commands using Spectre.Console in `src/Plotter.Cli/Presentation/Cli/WorkspaceCommandModule.cs`, `src/Plotter.Cli/Presentation/Cli/SceneCommandModule.cs`, and `src/Plotter.Cli/Presentation/Cli/EntityCommandModule.cs`; support `novel participant add`, `novel location add`, `novel scene add`, `novel scene set`, `novel scene list`, and `novel scene show`.
- [x] T032 [US1] Implement human-readable scene/entity output and actionable validation diagnostics in `src/Plotter.Cli/Presentation/Cli/TextRenderers.cs`.
- [x] T033 [US1] Make all US1 properties pass and verify `novel init`, `novel init --file`, the participant/location/scene initialization workflow, scene editing, persistence, and current-folder selection through `test/Plotter.PropertyTests/CliProperties/SceneCommandProperties.cs` and `test/Plotter.PropertyTests/CliProperties/WorkspaceCommandProperties.cs`.

**Checkpoint**: Scene context can be managed and persisted independently as the MVP.

---

## Phase 4: User Story 2 - Understand Story Timeline (Priority: P1)

**Goal**: Provide chronological and manuscript-order scene views with date ranges, Acts, chapters, and flashback markers.

**Independent Test**: Generate scenes with story and manuscript ordering, run each view, and verify ordering/grouping/classification invariants.

- [x] T034 [P] [US2] Define chronological ordering, inclusive date-range, unknown-time preservation, and tie-breaking properties in `test/Plotter.PropertyTests/ProjectionProperties/TimelineProperties.cs`.
- [x] T035 [P] [US2] Define manuscript-order Act/chapter grouping and flashback classification properties in `test/Plotter.PropertyTests/ProjectionProperties/ManuscriptOrderProperties.cs`.
- [x] T036 [US2] Implement shared timeline/manuscript query services in `src/Plotter.Cli/Application/Queries/TimelineQueries.cs`.
- [x] T037 [US2] Implement stable text timeline and manuscript renderers in `src/Plotter.Cli/Presentation/Cli/TimelineRenderers.cs`.
- [x] T038 [US2] Register `novel timeline`, `novel scenes timeline`, and `novel scene list --order manuscript` in `src/Plotter.Cli/Presentation/Cli/TimelineCommandModule.cs`.
- [x] T039 [US2] Add CLI property coverage for empty ranges, identical dates, missing dates, date bounds, and `[FLASHBACK]` output in `test/Plotter.PropertyTests/CliProperties/TimelineCommandProperties.cs`.

---

## Phase 5: User Story 3 - Analyze Character Continuity (Priority: P1)

**Goal**: Provide participant itineraries and multi-character lanes with complete occurrence/absence handling and filters.

**Independent Test**: Generate participant scenes across times/locations, run itinerary and lane queries, and verify every occurrence and empty cell is represented.

- [x] T040 [P] [US3] Define itinerary ordering, lane completeness, repeated same-time occurrence, empty-cell, group, character, and location-filter properties in `test/Plotter.PropertyTests/ProjectionProperties/CharacterContinuityProperties.cs`.
- [x] T041 [US3] Implement participant itinerary and lane query services in `src/Plotter.Cli/Application/Queries/CharacterContinuityQueries.cs`.
- [x] T042 [US3] Implement itinerary/lane text renderers in `src/Plotter.Cli/Presentation/Cli/CharacterRenderers.cs`.
- [x] T043 [US3] Register `novel character timeline/show` and `novel scenes lanes`/`novel lanes` commands in `src/Plotter.Cli/Presentation/Cli/CharacterCommandModule.cs`.
- [x] T044 [US3] Add property coverage for empty filters and participant-group membership in `test/Plotter.PropertyTests/CliProperties/CharacterCommandProperties.cs`.

---

## Phase 6: User Story 4 - Analyze Location Occupancy (Priority: P1)

**Goal**: Provide location timelines, density summaries, and point-in-time active-location queries.

**Independent Test**: Generate explicit-duration and duration-less scenes, query location occupancy and `where`, and verify explicit/inferred occupancy semantics.

- [x] T045 [P] [US4] Define explicit-duration precedence, duration-less next-scene inference, gaps/overlaps, no-location, and simultaneous-location conflict properties in `test/Plotter.PropertyTests/ProjectionProperties/LocationOccupancyProperties.cs`.
- [x] T046 [US4] Implement location timeline, density, and active-location query services in `src/Plotter.Cli/Application/Queries/LocationQueries.cs`.
- [x] T047 [US4] Implement location and `where` renderers in `src/Plotter.Cli/Presentation/Cli/LocationRenderers.cs`.
- [x] T048 [US4] Register `novel location show`, `novel location timeline`, `novel locations list --occupancy`, and `novel where` commands in `src/Plotter.Cli/Presentation/Cli/LocationCommandModule.cs`.
- [x] T049 [US4] Add CLI property coverage for occupancy ranges, first/last use dates, unknown locations, and conflicts in `test/Plotter.PropertyTests/CliProperties/LocationCommandProperties.cs`.

---

## Phase 7: User Story 5 - Analyze Plot-Thread Continuity (Priority: P1)

**Goal**: Provide a scene-by-thread matrix and per-thread timeline with classification symbols and annotations.

**Independent Test**: Generate plot-thread associations with all classifications and annotations, then verify matrix and timeline projections preserve all rows/threads.

- [x] T050 [P] [US5] Define plot-thread classification-symbol, matrix completeness, annotation ordering, unclassified-scene, and unresolved-thread properties in `test/Plotter.PropertyTests/ProjectionProperties/PlotThreadProperties.cs`.
- [x] T051 [US5] Implement plot-thread matrix and timeline query services in `src/Plotter.Cli/Application/Queries/PlotThreadQueries.cs`.
- [x] T052 [US5] Implement matrix and per-thread renderers in `src/Plotter.Cli/Presentation/Cli/PlotThreadRenderers.cs`.
- [x] T053 [US5] Register `novel threads matrix` and `novel thread show` commands in `src/Plotter.Cli/Presentation/Cli/PlotThreadCommandModule.cs`.
- [x] T054 [US5] Add CLI property coverage for `●`, `○`, `·`, `?`, empty rows, and missing resolutions in `test/Plotter.PropertyTests/CliProperties/PlotThreadCommandProperties.cs`.

---

## Phase 8: User Story 6 - Inspect Scene Detail Card (Priority: P1)

**Goal**: Make `novel scene show <id>` the detailed destination for scene inspection and continuity context.

**Independent Test**: Query a scene with adjacent scenes, plot threads, flags, and summary, then verify the complete card and missing-neighbor behavior.

- [x] T055 [P] [US6] Define scene-card completeness, previous/next resolution, elapsed-time, absent-field, and continuity-flag properties in `test/Plotter.PropertyTests/ProjectionProperties/SceneCardProperties.cs`.
- [x] T056 [US6] Implement scene detail query composition in `src/Plotter.Cli/Application/Queries/SceneDetailQueries.cs`.
- [x] T057 [US6] Implement structured scene-card renderer in `src/Plotter.Cli/Presentation/Cli/SceneCardRenderer.cs`.
- [x] T058 [US6] Route all scene-listing projections to the detail-card contract and register `novel scene show` in `src/Plotter.Cli/Presentation/Cli/SceneDetailCommandModule.cs`.
- [x] T059 [US6] Add property coverage for card projection equivalence and unknown elapsed time in `test/Plotter.PropertyTests/CliProperties/SceneCardCommandProperties.cs`.

---

## Phase 9: User Story 7 - Review Gaps and Audit Findings (Priority: P1)

**Goal**: Provide gap classification and focused/all audits with ERROR/WARN/INFO severity and intentional uncertainty handling.

**Independent Test**: Generate gaps, overlaps, missing fields, and continuity annotations, then verify classifications and severity.

- [x] T060 [P] [US7] Define gap boundary and severity/annotation properties in `test/Plotter.PropertyTests/ProjectionProperties/AuditProperties.cs`.
- [x] T061 [US7] Implement continuity gap calculations and Normal/Long/Overnight/Unknown classification in `src/Plotter.Cli/Application/Auditing/GapReportService.cs`.
- [x] T062 [US7] Implement time, travel, participant, location, and all audit rules in `src/Plotter.Cli/Application/Auditing/AuditService.cs`.
- [x] T063 [US7] Implement plain-text and SARIF audit renderers in `src/Plotter.Cli/Presentation/Cli/AuditRenderers.cs` and `src/Plotter.Cli/Presentation/Export/SarifExporter.cs`.
- [x] T064 [US7] Register `novel continuity gaps`, `novel audit`, and focused audit commands in `src/Plotter.Cli/Presentation/Cli/AuditCommandModule.cs`.
- [x] T065 [US7] Add property coverage proving intentional/approximate/implied annotations qualify findings without escalating every omission in `test/Plotter.PropertyTests/CliProperties/AuditCommandProperties.cs`.

---

## Phase 10: User Story 8 - Analyze Travel and Causality (Priority: P2)

**Goal**: Report available movement time and optional modeled routes without inventing travel durations.

**Independent Test**: Generate participant location sequences with and without travel models, then verify available-time and modeled-route behavior.

- [x] T066 [P] [US8] Define travel available-time, no-model, feasible-route, and no-route properties in `test/Plotter.PropertyTests/ProjectionProperties/TravelProperties.cs`.
- [x] T067 [US8] Implement available-time travel analysis in `src/Plotter.Cli/Application/Queries/TravelQueries.cs`.
- [x] T068 [US8] Implement optional coordinate/travel-time model adapters in `src/Plotter.Cli/Infrastructure/Travel/TravelModelAdapter.cs`.
- [x] T069 [US8] Implement travel text renderer and register `novel travel` in `src/Plotter.Cli/Presentation/Cli/TravelCommandModule.cs`.
- [x] T070 [US8] Add property coverage ensuring absent models never produce invented route durations in `test/Plotter.PropertyTests/CliProperties/TravelCommandProperties.cs`.

---

## Phase 11: User Story 9 - Track Character Interactions (Priority: P2)

**Goal**: Manage scene interactions and participant histories with reference validation.

**Independent Test**: Generate valid/invalid interaction records, persist them, and verify history and rejection invariants.

- [ ] T071 [P] [US9] Define interaction reference-closure, edit/remove, and history-ordering properties in `test/Plotter.PropertyTests/DomainProperties/InteractionProperties.cs`.
- [ ] T072 [US9] Implement interaction create-update-remove and participant history services in `src/Plotter.Cli/Application/Commands/InteractionCommands.cs` and `src/Plotter.Cli/Application/Queries/InteractionQueries.cs`.
- [ ] T073 [US9] Implement interaction CLI commands and renderers in `src/Plotter.Cli/Presentation/Cli/InteractionCommandModule.cs`.
- [ ] T074 [US9] Add property coverage for rejecting participants not assigned to a scene in `test/Plotter.PropertyTests/CliProperties/InteractionCommandProperties.cs`.

---

## Phase 12: User Story 10 - Organize Scenes by Plot (Priority: P2)

**Goal**: Manage Plot records, bounds, and scene-to-plot relationships.

**Independent Test**: Generate plots with valid/invalid bounds and scene associations, then verify persistence, listing, and rejection invariants.

- [ ] T075 [P] [US10] Define Plot ID, bound-order, scene-reference, and rename/remove properties in `test/Plotter.PropertyTests/DomainProperties/PlotProperties.cs`.
- [ ] T076 [US10] Implement Plot and scene-to-Plot application services in `src/Plotter.Cli/Application/Commands/PlotCommands.cs`.
- [ ] T077 [US10] Implement Plot CLI commands and renderers in `src/Plotter.Cli/Presentation/Cli/PlotCommandModule.cs`.
- [ ] T078 [US10] Add property coverage for rejecting empty/duplicate Plot IDs and end-before-start bounds in `test/Plotter.PropertyTests/CliProperties/PlotCommandProperties.cs`.

---

## Phase 13: User Story 11 - Share and Recover Novel Data (Priority: P2)

**Goal**: Provide safe portable TOML persistence, reopen/copy behavior, compatibility diagnostics, and explicit file selection.

**Independent Test**: Save a workspace, copy it to another folder, reopen it without network access, and verify canonical equivalence.

- [ ] T079 [P] [US11] Define workspace-copy, reopen, malformed-input, incompatible-version, and source-preservation properties in `test/Plotter.PropertyTests/PersistenceProperties/WorkspaceRecoveryProperties.cs`.
- [ ] T080 [US11] Implement format/version compatibility diagnostics and source-preserving load failures in `src/Plotter.Cli/Infrastructure/Storage/WorkspaceCompatibilityService.cs`.
- [ ] T081 [US11] Implement explicit `--file`/path selection and resolved-file diagnostics in `src/Plotter.Cli/Presentation/Cli/FileSelectionOptions.cs`.
- [ ] T082 [US11] Add end-to-end property coverage for copy/reopen/offline workflows in `test/Plotter.PropertyTests/CliProperties/WorkspaceCommandProperties.cs`.

---

## Phase 14: User Story 12 - Use Composable CLI, TUI, and Exports (Priority: P1)

**Goal**: Deliver stable command projections, machine-readable/visual exports, TUI navigation, and canonical-data equivalence.

**Independent Test**: Execute one query as text and structured formats, open it in the TUI, preserve selection while changing views, and export equivalent artifacts.

- [ ] T083 [P] [US12] Define text/JSON/CSV/SARIF equivalence and deterministic export properties in `test/Plotter.PropertyTests/ProjectionProperties/FormatEquivalenceProperties.cs`.
- [ ] T084 [P] [US12] Define CLI alias equivalence, empty-result validity, and current-folder file-selection properties in `test/Plotter.PropertyTests/CliProperties/ComposableCommandProperties.cs`.
- [ ] T085 [P] [US12] Define TUI selected-Scene-ID stability, filtered-selection fallback, keyboard-command, major-object CRUD, relationship-editing, and CLI/TUI service-equivalence properties in `test/Plotter.PropertyTests/CliProperties/TuiProperties.cs`.
- [ ] T086 [US12] Implement shared result DTO mapping and stable text/JSON/CSV renderers in `src/Plotter.Cli/Presentation/Export/BasicExporters.cs`.
- [ ] T087 [US12] Implement SARIF, Markdown, iCalendar, Graphviz DOT, Mermaid, HTML, SVG, and text exporters in `src/Plotter.Cli/Presentation/Export/ArtifactExporters.cs`.
- [ ] T088 [US12] Implement graph projection by participants/locations and register export commands in `src/Plotter.Cli/Presentation/Cli/ExportCommandModule.cs`.
- [ ] T089 [US12] Implement calendar day/week/month projections and commands in `src/Plotter.Cli/Presentation/Cli/CalendarCommandModule.cs`.
- [ ] T090 [US12] Implement the Spectre.Console TUI layout, filters, search, ordering/view controls, date jump, lanes, audit, quit, and Enter-to-detail behavior in `src/Plotter.Cli/Presentation/Tui/PlotterTui.cs`, including create/view/update/remove flows for Scenes, Participants, Locations, Plots, Participant Groups, and Interactions.
- [ ] T091 [US12] Implement the stable-selection state model and shared TUI query/command adapter in `src/Plotter.Cli/Presentation/Tui/TuiState.cs`, routing relationship and annotation edits through application services.
- [ ] T092 [US12] Add CLI/TUI/export property coverage for canonical-data equivalence, selection stability, major-object CRUD, relationship creation/removal, and shared validation in `test/Plotter.PropertyTests/CliProperties/InteractiveProjectionProperties.cs`.

---

## Phase 15: Polish & Cross-Cutting Concerns

**Purpose**: Package, document, benchmark, and validate the complete tool.

- [ ] T093 [P] Add package metadata, tool command name, README, license metadata, and NuGet packing settings in `src/Plotter.Cli/Plotter.Cli.csproj` and `README.md`.
- [ ] T094 [P] Write the getting-started guide covering installation, `novel init`, `novel init --file`, first scene attribution, and first continuity query in `docs/getting-started.md`.
- [ ] T095 [P] Write the detailed CLI/TUI command reference covering every supported command, option, output format, error behavior, and keyboard action in `docs/command-reference.md` and `docs/tui-guide.md`.
- [ ] T096 [P] Write task-oriented tutorials for scene management, timelines, character/location continuity, plot threads, audits, travel, TUI workflows, and exports in `docs/tutorials/`.
- [ ] T097 [P] Write developer documentation covering architecture, canonical TOML model, domain/application/presentation boundaries, query/projection contracts, dependency management, property invariants, TDD workflow, and contribution steps in `docs/developer-guide.md` and `docs/architecture.md`.
- [ ] T098 [P] Add XML documentation for public contracts and non-obvious failure/format behavior under `src/Plotter.Cli/`.
- [ ] T099 Add a repeatable benchmark harness and property-backed dataset for 1,000-scene chronological queries in `test/Plotter.PropertyTests/ProjectionProperties/PerformanceProperties.cs`, with a measured pass/fail threshold of under 2 seconds on the target personal-computer baseline.
- [ ] T100 Create `coverage.runsettings` and enforce 100% measured coverage for production code under `src/Plotter.Cli/`, excluding generated artifacts, failing CI on uncovered paths through `.github/workflows/ci.yml`.
- [ ] T101 Validate all documentation examples, command names, options, output formats, and TUI key descriptions against the CLI/TUI contracts in `test/Plotter.PropertyTests/RegressionTests/DocumentationContractChecks.md` and `docs/command-reference.md`.
- [ ] T102 Run `quickstart.md` and `docs/getting-started.md` end-to-end validation in a temporary workspace outside the repository and record any regression test IDs in `test/Plotter.PropertyTests/RegressionTests/`.
- [ ] T103 Run `dotnet format --verify-no-changes`, `dotnet build --warnaserror`, `dotnet test`, coverage, and NuGet pack validation using `.editorconfig`, `.github/workflows/ci.yml`, `src/Plotter.Cli/Plotter.Cli.csproj`, and `Directory.Packages.props` before release.

## Dependencies & Execution Order

### Phase dependencies

- **Phase 1 Setup**: No dependencies; establishes projects and package/tooling configuration.
- **Phase 2 Foundational**: Depends on Phase 1; blocks all user stories.
- **Phase 3 US1**: Depends on Phase 2; MVP and source-of-truth scene editing.
- **Phase 4 US2**: Depends on US1 scene/query contracts.
- **Phase 5 US3**: Depends on US1 and US2 query foundations; can proceed in parallel with US4/US5 after shared query contracts stabilize.
- **Phase 6 US4**: Depends on US1 scene/location data; can proceed in parallel with US3/US5.
- **Phase 7 US5**: Depends on US1 Plot relationship data; can proceed in parallel with US3/US4.
- **Phase 8 US6**: Depends on US1 and the query contracts from US2–US5.
- **Phase 9 US7**: Depends on temporal/location/participant query services from US2–US4.
- **Phase 10 US8**: Depends on US4 occupancy semantics and optional travel model contracts.
- **Phase 11 US9**: Depends on US1 participant/scene references; can proceed in parallel with US10.
- **Phase 12 US10**: Depends on US1 Plot references; can proceed in parallel with US9.
- **Phase 13 US11**: Depends on foundational persistence and all entities needed for full workspace recovery.
- **Phase 14 US12**: P1 priority but intentionally scheduled after the prerequisite query stories because the required TUI and exporters project their stabilized contracts; TUI/export slices can proceed in parallel after those contracts stabilize.
- **Phase 15 Polish**: Depends on all selected stories and their properties passing.

### User story completion order

```text
Foundation → US1 → {US2, US3, US4, US5, US9, US10}
{US2, US3, US4, US5} → US6
{US2, US3, US4} → US7 → US8
All entity/persistence stories → US11
{US2..US11 query contracts} → US12
All selected stories → Polish
```

## Requirements Traceability

| Requirement range | Primary task coverage            |
| ----------------- | -------------------------------- |
| FR-001–FR-006     | T009–T033                        |
| FR-007–FR-008     | T034–T039                        |
| FR-009–FR-010e    | T040–T044, T071–T074             |
| FR-010f–FR-010i   | T050–T054                        |
| FR-010j–FR-010k   | T055–T059                        |
| FR-011a–FR-011e   | T045–T049                        |
| FR-011f–FR-011i   | T060–T065                        |
| FR-011j–FR-011k   | T066–T070                        |
| FR-012–FR-013     | T011, T017–T019, T079–T082       |
| FR-014–FR-015     | T031–T032, T083–T092             |
| FR-016            | T003, T093, T099                 |
| FR-017            | T015, T017, T079–T082            |
| FR-018–FR-021a    | T083–T092, T103                  |
| FR-022–FR-025     | T094–T102                        |
| SC-001–SC-020     | Story property suites, T099–T103 |
| SC-021–SC-023     | T094–T102                        |

## Parallel execution examples

### After Phase 2

```text
T034/T035  Timeline and manuscript-order properties
T040       Character continuity properties
T045       Location occupancy properties
T050       Plot-thread properties
T071       Interaction properties
T075       Plot properties
```

These use different test files and can be authored in parallel once foundational generators/oracles exist.

### US12 projection work

```text
T083  Format equivalence properties
T084  CLI alias/empty-result properties
T085  TUI selection properties
T087  Artifact exporters
T089  Calendar projections
```

These can proceed in parallel after shared DTO/query contracts are available.

## Implementation strategy

### MVP

1. Complete Phase 1 and Phase 2.
2. Complete US1 scene/entity context management.
3. Validate the MVP with property-based persistence, validation, and CLI command properties.
4. Package a local/global NuGet tool only after the MVP passes the release gates.

### Incremental delivery

1. Add US2 chronological/manuscript views.
2. Add US3–US5 continuity projections.
3. Add US6 scene detail card and US7 audits.
4. Add US8 travel, US9 interactions, US10 plot management, and US11 recovery/file selection.
5. Add US12 TUI/export/calendar capabilities over the stabilized query layer.
6. Complete polish, performance, coverage, and package validation.

## Notes

- Every task uses the required checklist format: checkbox, sequential ID, optional `[P]`, required story label in story phases, and an exact file path.
- Tests are property-based by default and must be written before implementation. Regression examples require a named prior defect.
- No unit-test-only phase is included because the user and constitution require property-based coverage except for regressions.
- The task list deliberately keeps TUI/export work behind shared query contracts so projections cannot diverge from canonical TOML data.
