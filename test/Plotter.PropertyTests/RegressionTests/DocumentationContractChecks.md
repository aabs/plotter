# Documentation Contract Checks

User and developer documentation must reflect the actual CLI/TUI contracts:

1. Every documented top-level command noun must map to an implemented command.
2. Every documented option, output format, and TUI key must match the supported surface.
3. Documentation examples must be runnable against the current CLI.
4. Exports described in documentation must be produced by the export command modules.

`DocumentationContractChecks.cs` enforces these rules as property tests against `docs/command-reference.md` and `docs/getting-started.md`. Keep the reference documentation in sync with the implemented command vocabulary.
