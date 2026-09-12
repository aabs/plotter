# Required Property Invariants

These invariants guide the property-based test plan; each must have a generator, oracle, and shrinkable counterexample.

1. **ID uniqueness**: Valid workspaces never contain duplicate Scene, Participant, Location, or Plot IDs.
2. **Reference closure**: Every persisted reference resolves to an entity in the same workspace.
3. **Persistence round trip**: Save(load(save(workspace))) is equivalent to the original canonical workspace.
4. **Atomic invalid writes**: Rejected edits leave the prior persisted file unchanged.
5. **Chronological ordering**: Story-time projections are ordered by known date/time and preserve unknown/partial entries without silently dropping them.
6. **Manuscript ordering**: Manuscript projections preserve narrative-position ordering, Act/chapter grouping, and flashback classification.
7. **Projection equivalence**: Text, JSON, CSV, and applicable export records describe the same query result IDs and classifications.
8. **Audit severity**: Intentional/approximate/implied annotations qualify findings without converting valid uncertainty into hard errors.
9. **Gap classification**: Gap class is determined solely by the elapsed-time boundary or missing data according to the contract.
10. **Lane completeness**: Character lanes preserve every matching scene occurrence and represent absent participants explicitly.
11. **Occupancy semantics**: Explicit duration takes precedence; duration-less participant occupancy ends at that participant's next known scene.
12. **TUI selection stability**: View/order changes preserve the selected Scene ID whenever it remains in the result set.
13. **Export determinism**: Same canonical input and query options produce equivalent deterministic export output.
14. **Path resolution**: Default current-folder resolution and explicit file selection resolve to the intended workspace without cross-file leakage.
