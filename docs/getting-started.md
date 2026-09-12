# Getting Started with Plotter

## Prerequisites

- .NET 10 SDK or runtime.
- Plotter installed as a global tool (`dotnet tool install -g Plotter.Cli`).

## Initialize a novel workspace

```bash
mkdir my-novel
cd my-novel
novel init
```

This creates `novel.toml` in the current folder and reports the resolved file. To create or open a workspace at an explicit path:

```bash
novel init --file ~/novels/my-novel.toml
```

## Record a scene

```bash
novel participant add Mara
novel location add "Boarding house"
novel scene add S001
novel scene set S001 \
  --date-time 1928-06-14T08:10 \
  --participant Mara \
  --location "Boarding house" \
  --title "Mara wakes"
```

Inspect the scene:

```bash
novel scene show S001
```

## Run your first continuity query

```bash
novel timeline
novel character timeline Mara
novel audit time
```

All commands operate on the `novel.toml` in the current folder by default. Use `--file PATH` anywhere to select a different workspace.
