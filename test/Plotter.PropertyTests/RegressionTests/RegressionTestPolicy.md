# Regression Test Policy

Example-based tests are permitted only for specific regression defects that previously caused issues. All broad correctness coverage must use property-based tests (FsCheck/FsCheck.Xunit) with explicit invariants and oracles.

A regression test must:

1. Reference the concrete defect it protects (issue/task identifier or a precise description).
2. Be added only after a failing property or scenario has been observed.
3. Remain narrow: it documents the specific historical failure rather than replacing property coverage.

All other behavior is covered by property-based tests with generators, shrinking, and mechanical or reference-model oracles.
