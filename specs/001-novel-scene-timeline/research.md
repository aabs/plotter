# Research: Novel Scene Timeline

## Decision 1: Target framework and language

- **Decision**: Target .NET 10 with C# 14.
- **Rationale**: Explicit user constraint and alignment with the repository constitution (`CFT-01`). Nullable reference types and analyzers remain enabled.
- **Alternatives considered**: Earlier .NET versions were rejected because they conflict with the project constraint; multi-targeting was rejected for the initial release to keep packaging and test matrices small.

## Decision 2: CLI framework

- **Decision**: Use Spectre.Console for command-line interaction and terminal rendering.
- **Rationale**: It provides command parsing, tables, markup, prompts, and terminal capabilities while keeping human-readable output consistent. Core query services remain independent of the presentation layer.
- **Alternatives considered**: `System.CommandLine` alone was rejected because the requested TUI and terminal presentation require richer rendering; a custom parser was rejected as unnecessary complexity.

## Decision 3: Canonical storage

- **Decision**: Use one human-readable TOML data file per novel workspace. Resolve the active file from the current working directory by default, with an explicit file/path override for multiple novel files.
- **Rationale**: This satisfies portability, Git diffs, offline use, and the requirement to work with multiple novel files. Scene, participant, location, plot, group, and relationship IDs are stable references in TOML.
- **Alternatives considered**: A database was rejected for portability and diffability; JSON/YAML were rejected because TOML is an explicit requirement.

## Decision 4: TOML library

- **Decision**: Use Tomlyn as the TOML parser/serializer, wrapped behind an application storage interface.
- **Rationale**: It provides TOML-focused .NET serialization while keeping the rest of the application independent of the package API. The wrapper allows format validation, DTO mapping, and future parser replacement without leaking library types into the domain.
- **Alternatives considered**: Writing a TOML parser was rejected as unnecessary risk; binding the domain directly to a parser package was rejected by the serialization-boundary constitution rule.

## Decision 5: CLI and projection architecture

- **Decision**: Separate domain/storage, query/projection, command orchestration, and presentation/export layers. CLI commands, TUI views, and artifact exporters consume the same query contracts.
- **Rationale**: Prevents the TUI or exports from becoming alternate data models and guarantees equivalent results across plain text, JSON, CSV, SARIF, and visual/document formats.
- **Alternatives considered**: Implementing each view directly against TOML was rejected because it would duplicate business rules and make consistency difficult to test.

## Decision 6: Testing strategy

- **Decision**: Use FsCheck and FsCheck.Xunit for property-based tests as the default and required test style. Add only narrowly scoped regression tests for known defects.
- **Rationale**: This is mandated by the constitution and the user requirement. Properties will cover round-trip persistence, ID/reference invariants, ordering, filtering, audit severity, export equivalence, and projection consistency.
- **Alternatives considered**: Example-only unit tests were rejected. Traditional unit tests are permitted only when documenting a specific regression.

## Decision 7: Package management

- **Decision**: Enable central package management through a repository-level `Directory.Packages.props`; package versions are declared centrally and project files reference packages without versions.
- **Rationale**: Explicit user requirement and consistent dependency versioning across production, test, and tooling projects.
- **Alternatives considered**: Per-project package versions were rejected because they permit drift.

## Decision 8: Active novel file resolution

- **Decision**: Default to the novel data file in the current folder. Provide an explicit `--file`/path option and report the resolved file in diagnostics/help when useful.
- **Rationale**: Keeps the common workflow short while supporting multiple novels and scripts operating in separate folders.
- **Alternatives considered**: A global config-selected file was rejected because it is surprising and unsafe when switching projects.

## Decision 9: Time and continuity semantics

- **Decision**: Preserve unknown, approximate, implied, and intentional values explicitly. Use explicit duration where available; use the existing participant-next-scene inference only for views that require an active-location estimate. Audits classify findings as ERROR, WARN, or INFO rather than treating all omissions as errors.
- **Rationale**: Fiction routinely contains intentional uncertainty and simultaneity. The tool must expose evidence without inventing certainty.
- **Alternatives considered**: Strict validation of every missing value was rejected because it would make legitimate fiction impossible to represent.

## Decision 10: Export formats

- **Decision**: Implement JSON and CSV as initial structured exports, SARIF for audit output, and Markdown/text for human documentation. Define iCalendar, Graphviz DOT, Mermaid, HTML, and SVG as export contracts and staged implementation targets.
- **Rationale**: JSON/CSV/SARIF directly support scripting and tooling; other formats support the stated editor, calendar, graph, and documentation workflows.
- **Alternatives considered**: Making every format a first-release blocker was rejected to keep the initial implementation deliverable; the contracts remain explicit for compatible follow-up work.

## Decision 11: TUI scope

- **Decision**: Treat the TUI as a projection/browser over query services, with stable Scene ID selection and keyboard navigation. Keep editing commands routed through the same application services as the CLI.
- **Rationale**: Ensures a selected scene remains meaningful while changing views and avoids data divergence.
- **Alternatives considered**: A separate TUI data model was rejected because it would violate the canonical-data requirement.
