# Implementation Plan: Novel Scene Timeline

**Branch**: `001-novel-scene-timeline` | **Date**: 2026-09-12 | **Spec**: [spec.md](spec.md)

## Summary

Build a .NET 10 / C# 14 NuGet CLI tool for managing novel scenes and continuity projections. TOML is the canonical, human-readable storage format. The active novel file is resolved from the current folder by default, with explicit file selection for multiple novel workspaces. Spectre.Console provides command-line parsing/rendering and the TUI. CLI commands, TUI views, audits, and exports consume shared domain/query contracts rather than maintaining separate models. FsCheck and FsCheck.Xunit property-based tests are the default test mechanism, with 100% production-code coverage and narrowly scoped regression tests only when needed.

## Technical Context

**Language/Version**: C# 14 on .NET 10 (`net10.0`)

**Primary Dependencies**: Spectre.Console; Tomlyn behind a storage abstraction for TOML; FsCheck and FsCheck.Xunit for property-based tests; .NET testing platform/xUnit integration required by the constitution.

**Storage**: Local human-readable TOML file; `novel.toml` in the current folder by default, with an explicit path override for multiple novels.

**Testing**: FsCheck + FsCheck.Xunit property-based tests; TDD red-green-refactor; regression-only example tests; coverage collection with a 100% production-code gate.

**Target Platform**: Supported desktop environments with the .NET 10 runtime/SDK; offline operation for local workspace operations.

**Project Type**: NuGet-distributed CLI tool with a required initial-release interactive TUI and derived exporters.

**Performance Goals**: Chronological query over 1,000 scenes under 2 seconds on a typical personal computer, verified by a repeatable benchmark; exports and audits should operate from one loaded workspace snapshot.

**Constraints**: TOML remains canonical; current-folder resolution is the default; stable plain-text output; machine formats must preserve equivalent results; no external service required; TUI is a required projection over shared query services; central package management; warnings as errors/analyzers; immutable-first domain models; async I/O with CancellationToken propagation; 100% measured production-code coverage excluding generated artifacts; property-based tests by default.

**Scale/Scope**: Individual novelists/editors; local workspaces; initial target of at least 1,000 scenes per workspace; no collaboration or concurrent multi-writer editing in the first release.

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- **CFT-01 through CFT-09**: PASS. Production target is `net10.0`; nullable/analyzers/warnings-as-errors and narrow visibility are design requirements.
- **CAR-01 through CAR-03, PERF-17, and PERF-20**: PASS. File, query, export, and TUI operations that perform cancellable I/O use async APIs and propagate CancellationToken without blocking.
- **PBT-01, PBT-03 through PBT-12**: PASS. FsCheck/FsCheck.Xunit properties, generators, shrinking, explicit invariants/oracles, deterministic inputs, and regression-only example tests are required.
- **TDD-001 through TDD-004**: PASS. Implementation tasks must introduce a failing property before production behavior.
- **CDB-01 through CDB-06**: PASS. Storage, query, formatting, and presentation boundaries use abstractions, DI, options, and explicit DTOs.
- **CDM-01 through CDM-12**: PASS. Domain records/value objects are immutable-first and framework-independent; presentation stays at boundaries.
- **CEL-01 through CEL-10**: PASS. Boundary validation, explicit expected-result errors, diagnostics preservation, and commit-or-rollback file writes are included.
- **AI-001/AI-002**: PASS. Temporary test/diagnostic artifacts remain outside the repository.
- **UTR-001**: PASS planned through CLI/TUI integration properties and end-to-end quickstart scenarios.

## Phase 0: Research Summary

Research is captured in [research.md](research.md). Key decisions are .NET 10/C# 14, Spectre.Console, TOML current-folder resolution, shared projections, central package management, FsCheck property testing, and the complete export contract required by the specification.

## Phase 1: Design Summary

- [data-model.md](data-model.md) defines workspace entities, IDs, relationships, lifecycle, and invariants.
- [contracts/cli.md](contracts/cli.md) defines command vocabulary, initialization workflow, output formats, audit/gap contracts, and export boundaries.
- [contracts/tui.md](contracts/tui.md) defines shared-query behavior, layout responsibilities, keyboard interactions, and stable selection.
- [quickstart.md](quickstart.md) defines end-to-end validation and release checks.

## Project Structure

### Documentation

```text
specs/001-novel-scene-timeline/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── cli.md
│   ├── tui.md
│   ├── formats.md
│   └── domain-properties.md
└── tasks.md                 # generated by /speckit-tasks
```

### Source Code

```text
src/
└── Plotter.Cli/
    ├── Plotter.Cli.csproj
    ├── Program.cs
    ├── Domain/
    ├── Application/
    │   ├── Commands/
    │   ├── Queries/
    │   ├── Auditing/
    │   └── Projections/
    ├── Infrastructure/
    │   ├── Storage/
    │   ├── Configuration/
    │   └── Packaging/
    ├── Presentation/
    │   ├── Cli/
    │   ├── Tui/
    │   └── Export/
    └── Serialization/

test/
└── Plotter.PropertyTests/
    ├── Plotter.PropertyTests.csproj
    ├── Generators/
    ├── Oracles/
    ├── DomainProperties/
    ├── PersistenceProperties/
    ├── ProjectionProperties/
    ├── CliProperties/
    └── RegressionTests/

Directory.Packages.props
.editorconfig
```

**Structure Decision**: Keep the existing executable project as the application entry point, organize production code by domain/application/infrastructure/presentation boundaries, and add one property-test project plus a narrowly scoped regression folder. A separate shared library is not required until reuse or packaging boundaries justify it.

## Implementation sequencing notes

1. Establish central package management, project analyzers, package metadata, and file-resolution options. The initialization command set is `novel init` (current-folder `novel.toml`), `novel init --file <path>`, `novel participant add <name>`, `novel location add <name>`, `novel scene add <scene-id>`, `novel scene set <scene-id> [options]`, `novel scene list`, and `novel scene show <scene-id>`.
2. Define immutable domain records, IDs, temporal values, continuity annotations, and validation results.
3. Implement TOML DTOs and atomic load/save services with current-folder/default-file resolution.
4. Implement shared query services for chronological, manuscript, character, location, plot-thread, gap, audit, and travel projections.
5. Implement all specified stable text/JSON/CSV/SARIF/Markdown/iCalendar/Graphviz DOT/Mermaid/HTML/SVG projections and export boundaries.
6. Implement Spectre.Console command registration and the required TUI over the shared queries. The TUI must create, view, update, and remove Scenes, Participants, Locations, Plots, Participant Groups, and Interactions, plus supported relationships and annotations through shared application services.
7. Add NuGet tool packaging and help/quickstart documentation.
8. Verify repeatable performance and measured coverage gates before release.
9. Drive each slice test-first with FsCheck properties and enforce coverage/analyzer gates.

## Complexity Tracking

No constitution violations are proposed. The TUI and exporter layers are justified because they are explicit user requirements and remain thin projections over shared application contracts.
