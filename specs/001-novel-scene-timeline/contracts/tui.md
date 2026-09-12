# TUI Contract

## Layout

The TUI presents a scene list, detail pane, and filter/help/status area. It is a browser over application query services, not a second data model.

## Interaction

- `j`/`k`: move selection.
- `Enter`: open the selected scene detail card.
- `/`: search.
- `f`: filters.
- `o`: ordering.
- `v`: view selection.
- `g`: date navigation.
- `c`: character lanes.
- `a`: audit.
- `e`: edit through application commands/services.
- `q`: quit.

The selected Scene ID remains stable through view/order/filter changes while it remains in the result set. If filtered out, the TUI communicates that state and provides a deterministic fallback selection.
