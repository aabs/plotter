# Plotter

A .NET 10 / C# 14 command-line tool for managing the dates, times, participants, locations, plots, and continuity of scenes in a novel. Plotter is distributed as a NuGet tool and works entirely offline against a human-readable TOML workspace file.

## Getting started

```bash
mkdir my-novel
cd my-novel
novel init

novel participant add Mara
novel location add "Boarding house"
novel scene add S001
novel scene set S001 --date-time 1928-06-14T08:10 --participant Mara --location "Boarding house"
novel scene show S001
```

The default workspace is `novel.toml` in the current folder. Use `--file PATH` on any command (or `novel init --file PATH`) to work with another novel workspace.

## Views

- `novel timeline` / `novel scenes timeline --from DATE --to DATE` — chronological scene list
- `novel scene list --order manuscript` — Act/chapter narrative order with `[FLASHBACK]` markers
- `novel character timeline <name>` and `novel scenes lanes --characters a,b,c` — character continuity
- `novel location show <location>`, `novel locations list --occupancy`, `novel where [name] --at TIME` — location occupancy
- `novel threads matrix` and `novel thread show <plot>` — plot-thread analysis
- `novel continuity gaps` and `novel audit [time|travel|participants|locations|all]` — continuity audits
- `novel travel <name>` — available movement time
- `novel calendar --day|--week|--month --date DATE` — calendar projection
- `novel export --format json|csv|markdown|ical|html|svg|dot|mermaid|text` and `novel export graph --by participants|locations` — derived artifacts
- `novel tui` — interactive browser over the same queries

## Data

Workspaces are stored as TOML. Scenes, participants, locations, plots, and groups are keyed by stable non-empty IDs; scenes reference entities by ID. All exports are derived projections of the canonical workspace.

## Development

- Target: `net10.0`, C# 14, nullable enabled, warnings as errors
- Central package management via `Directory.Packages.props`
- Property-based tests with FsCheck/FsCheck.Xunit; example tests are reserved for regression defects
- See `docs/developer-guide.md` and `docs/architecture.md` for the architecture and contribution workflow
