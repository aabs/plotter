# Quickstart Validation Guide

## Prerequisites

- .NET 10 SDK.
- A built Plotter CLI package or local project.
- A temporary working directory; `novel init` creates `novel.toml` there, or an explicit file path may be selected.

## Validate the primary workflow

1. Run `mkdir my-novel`, `cd my-novel`, and `novel init`; verify that `novel.toml` is created and reported.
2. Run `novel init --file ~/novels/my-novel.toml` in a separate folder and verify that the explicit workspace is selected without changing the current-folder default.
3. Run `novel participant add Mara`, `novel location add "Boarding house"`, and `novel scene add S001`.
4. Run `novel scene set S001 --date-time 1928-06-14T08:10 --participant Mara --location "Boarding house"`.
5. Run `novel scene list --order manuscript` and verify Act/chapter/narrative ordering.
6. Run `novel timeline --order story-time` and verify chronological grouping.
7. Run `novel scene show <scene-id>` and verify the detail card links adjacent scenes and reports flags.
8. Run `novel character timeline <participant>` and verify itinerary output.
9. Run `novel audit time` and `novel audit participants`; verify ERROR/WARN/INFO severity.
10. Run `novel export --format json` and `novel export --format csv`; compare records with the TOML source.
11. Start the TUI, select a scene, switch views, and press Enter; verify the same Scene ID opens in the detail pane.

## Validate continuity projections

- `novel location show "Hotel ballroom" --timeline`
- `novel where --at 1928-06-14T15:00`
- `novel threads matrix`
- `novel continuity gaps`
- `novel travel Mara --date 1928-06-14`

## Test commands

- `dotnet test` runs the property-based test suite.
- `dotnet test --collect:"XPlat Code Coverage"` collects coverage.
- The release gate requires 100% measured coverage for the production code scope and all required properties to pass.

Each property test documents its invariant and oracle. Example-based tests are permitted only for named regression defects.

## Package and format checks

- Build with warnings as errors and analyzers enabled.
- Verify package versions are declared centrally in `Directory.Packages.props`.
- Verify CLI help documents the current-folder default and explicit file selection.
- Verify export output is derived from the same query result as human-readable output.
