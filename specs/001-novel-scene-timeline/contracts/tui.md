# TUI Contract

## Layout

The TUI presents a scene list, detail pane, and filter/help/status area. It is a browser and editor over application query/command services, not a second data model.

The TUI must provide create, view, update, and remove actions for Scenes, Participants, Locations, Plots, Participant Groups, and Interactions. It must also provide relationship/annotation actions for scene-to-participant, scene-to-location, scene-to-plot, POV participant, plot-thread classification/annotation, participant-group membership, and continuity annotations.

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

All TUI create/update/remove actions use the same validation, persistence, and application services as equivalent CLI commands.
