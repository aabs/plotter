# Output Format Contract

## Canonical equivalence

For the same workspace and query, human-readable output and structured formats must represent the same result set, ordering, IDs, and severity/classification values. Presentation-only details may differ.

## Formats

- `text`: stable terminal-friendly plain text; default.
- `json`: structured query/result DTOs suitable for scripts and editor integrations.
- `csv`: tabular records with stable headers and one logical record per row.
- `sarif`: audit findings mapped to SARIF results and levels.
- `markdown`: documentation-friendly tables/sections.
- `ical`: fictional story-world events as calendar entries where time data is sufficient.
- `dot`: Graphviz graph of participant or location relationships.
- `mermaid`: diagram source derived from relationships/timelines.
- `html`: standalone human-readable report.
- `svg`: terminal-friendly/visual report where the projection supports it.

Empty result behavior is format-valid and explicit; exporters never mutate TOML.
