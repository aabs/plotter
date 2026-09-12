# TUI Guide

`novel tui` is an interactive browser over the same queries and services as the CLI.

## Layout

The TUI presents a scene list, a detail view, and prompts for navigation. It never maintains a separate data model.

## Keys and actions

- Use the arrow keys or `j`/`k` to move between scenes.
- Press Enter to confirm a selection.
- `details` — open the selected scene's detail card.
- `edit` — route edits through the same application services as the CLI.
- `q` / `quit` — exit the TUI.

## Stable selection

The selected scene remains stable across view, ordering, and filter changes while it remains in the result set. If the selection is filtered out, the TUI selects the first remaining scene deterministically.

## Scope

The TUI provides create/view/update/remove flows for Scenes, Participants, Locations, Plots, Participant Groups, and Interactions through the shared application services.
