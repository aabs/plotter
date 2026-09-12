# Quickstart Validation Guide

## Prerequisites

- .NET 10 SDK.
- A built Plotter CLI package or local project.
- A TOML novel file in the working directory, or an explicit file path.

## Validate the primary workflow

1. Initialize or place a TOML novel file in a temporary working directory.
2. Run `novel scene list --order manuscript` and verify Act/chapter/narrative ordering.
3. Run `novel timeline --order story-time` and verify chronological grouping.
4. Run `novel scene show <scene-id>` and verify the detail card links adjacent scenes and reports flags.
5. Run `novel character timeline <participant>` and verify itinerary output.
6. Run `novel audit time` and `novel audit participants`; verify ERROR/WARN/INFO severity.
7. Run `novel export --format json` and `novel export --format csv`; compare records with the TOML source.
8. Start the TUI, select a scene, switch views, and press Enter; verify the same Scene ID opens in the detail pane.

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
