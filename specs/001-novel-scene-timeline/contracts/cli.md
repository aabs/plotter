# CLI Contract

## General rules

- Commands read `novel.toml` from the current folder by default.
- `novel init` creates or opens the current-folder `novel.toml` without replacing existing data.
- `novel init --file <path>` creates or opens an explicitly selected workspace without changing the current-folder default.
- An explicit file/path option selects another novel workspace.
- Human-readable plain text is the default and must be stable for Git diffs.
- `--format json|csv|sarif|markdown|ical|dot|mermaid|html|svg|text` is explicit; unsupported combinations fail with an actionable diagnostic.
- Expected empty results are successful and clearly represented.
- Invalid input and unresolved references return actionable diagnostics without changing the source file.

## Initial command vocabulary

```text
novel init
novel init --file <path>
novel participant add <name>
novel location add <name>
novel scene add <scene-id>
novel scene set <scene-id> [options]
novel scene list [--order manuscript|story-time] [filters]
novel scene show <scene-id>
novel timeline [--order story-time] [--from DATE] [--to DATE] [--character NAME]
novel character timeline <participant> [--date DATE]
novel location timeline <location>
novel thread show <plot-thread>
novel threads matrix
novel audit [time|travel|participants|locations|all]
novel continuity gaps
novel travel <participant> --date DATE
novel calendar --day|--week|--month <period>
novel export --format FORMAT
novel export graph --by participants|locations
```

## Scene show contract

The result contains Scene ID, title/summary, manuscript position, story time, location, POV, participants, plot threads, previous/next scene, elapsed time, flags, and notes/summary when present.

## Audit contract

Each finding contains severity, stable finding identifier, affected Scene IDs/entities, message, and relevant evidence. SARIF maps findings to SARIF results while preserving severity and locations where available.

## Continuity gaps contract

Each row contains source Scene ID, destination Scene ID, source/destination context, elapsed gap, and GapClass.

## Export contract

Exporters consume query result DTOs or artifact-specific projection DTOs; they must not read TOML independently or mutate canonical data.
