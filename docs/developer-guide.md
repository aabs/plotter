# Developer Guide

## Stack

- .NET 10 / C# 14, nullable enabled, analyzers on, warnings as errors.
- Central package management through `Directory.Packages.props`.
- Spectre.Console for CLI rendering and the TUI.
- Tomlyn for TOML serialization behind `INovelWorkspaceStore`.
- FsCheck / FsCheck.Xunit for property-based tests.

## Testing policy

- Property-based tests are the default. Each property defines a universal invariant with a mechanical or reference-model oracle.
- Example-based tests are reserved for specific regression defects and must reference the defect they protect (see `test/Plotter.PropertyTests/RegressionTests/RegressionTestPolicy.md`).
- Use TDD: write the failing property first, observe it fail, then implement.

## Architecture boundaries

- `Domain` — immutable entities, identifiers, values, validation, and diagnostics.
- `Application` — command services, query services, auditing, and projection contracts.
- `Infrastructure` — storage, configuration, diagnostics, travel model, and composition.
- `Presentation` — CLI command modules, renderers, exporters, and the TUI.
- `Serialization` — explicit TOML DTOs.

## Contribution workflow

1. Run `dotnet restore`.
2. Write a failing property-based test.
3. Implement the smallest change that makes it pass.
4. Refactor, keeping the suite green.
5. Run `dotnet build`, `dotnet test`, and `dotnet format --verify-no-changes` before submitting.
