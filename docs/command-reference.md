# Command Reference

All commands read the active workspace (the `novel.toml` in the current folder unless `--file PATH` is given) and produce stable plain-text output by default.

## Workspace

- `novel init [--file PATH]` — create or open a workspace.

## Entities

- `novel participant add <name>`
- `novel location add <name>`
- `novel scene add <scene-id>`
- `novel scene set <scene-id> [--date-time T] [--participant ID] [--location ID] [--title T] [--status S] [--notes N]`
- `novel plot add <id> [--description D] [--start T] [--end T]`
- `novel plot remove <id>`
- `novel interaction add <scene-id> [--type T] [--description D] [--participant A,B]`
- `novel interaction history <participant>`

## Views

- `novel scene list [--order manuscript|story-time]`
- `novel scene show <scene-id>`
- `novel timeline [--from DATE] [--to DATE] [--order manuscript]`
- `novel scenes timeline [--from DATE] [--to DATE]`
- `novel character timeline <name>`
- `novel scenes lanes --characters a,b,c [--date DATE]`
- `novel lanes [--characters a,b,c] [--character X] [--group G] [--location L] [--date DATE]`
- `novel location show <location>`
- `novel locations list --occupancy`
- `novel where [participant] --at YYYY-MM-DDTHH:mm`
- `novel threads matrix`
- `novel thread show <plot-thread>`
- `novel continuity gaps`
- `novel audit [time|travel|participants|locations|all]`
- `novel travel <participant>`
- `novel calendar --day|--week|--month [--date DATE]`

## Export

- `novel export --format text|json|csv|markdown|ical|html|svg|dot|mermaid`
- `novel export graph --by participants|locations [--format dot|mermaid]`

## Interactive

- `novel tui` — browse scenes, open details with Enter, and quit with `q`.

## Errors

Invalid commands, missing values, invalid dates/times, and unknown identifiers produce actionable `ERROR` messages on stderr without modifying the workspace file.
