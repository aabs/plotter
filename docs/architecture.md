# Architecture

## Overview

Plotter is a CLI tool that projects one canonical workspace into many views. The canonical data lives in a human-readable TOML file; every query, audit, export, and TUI view is a derived projection over the same loaded workspace.

## Canonical data model

- `NovelWorkspace` holds `Participants`, `Locations`, `Plots`, `ParticipantGroups`, and `Scenes`, each keyed by stable non-empty IDs.
- `Scene` carries narrative position, Act, chapter, story time, duration, location, participants, plots (with classification/annotation), POV participant, status, notes, continuity annotations, and interactions.
- Invariants: non-empty unique IDs, reference closure, POV among participants, non-negative duration, and plot end-not-before-start.

## Storage

`TomlWorkspaceStore` implements `INovelWorkspaceStore` using Tomlyn, with format/version validation and atomic temp-file writes. `NovelFileResolver` resolves the current-folder `novel.toml` by default or an explicit `--file`.

## Query and projection layer

`Application.Queries` exposes timeline, manuscript, character-continuity, location-occupancy, plot-thread, scene-detail, travel, and calendar queries over the workspace. Presentation modules render them; exporters project them into text, JSON, CSV, Markdown, iCalendar, Graphviz DOT, Mermaid, HTML, and SVG. The TUI uses the same query adapter.

## Auditing

`Application.Auditing` computes gap classes and audit findings (ERROR/WARN/INFO). Continuity annotations (time-confidence, travel-status, continuity-status) qualify findings so intentional uncertainty is not escalated.

## Projection rule

All projections derive from the same loaded workspace. No projection introduces a separate editable model, and no exporter mutates canonical data.
