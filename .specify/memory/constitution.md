# Engineering Constitution
## Agentic
- AI-001 [MANDATORY]: Never leave temporary files in the repo after they are no longer in use.
- AI-002 [MANDATORY]: When developing diagnostic, experimental or temporary test code, make sure it is created and run somewhere other than the source repo.  This prevents dead code being left lying around.
## Development
- CAR-01 [MANDATORY]: Use `async` and `await` for naturally asynchronous operations and return `Task` or `Task<T>` by default.
- CAR-02 [MANDATORY]: Accept a `CancellationToken` in cancellable async APIs and pass it through to all cancellable downstream calls.
- CAR-03 [MANDATORY]: Do not block async flows with `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`; await tasks instead.
- CAR-04: Use `ValueTask` only in measured hot paths where synchronous completion is common and allocation reduction is proven.
- CAR-05 [MANDATORY]: Use clear LINQ and collection code by default, but materialize sequences when needed to avoid hidden multiple enumeration (for example, `ToList()` before multiple passes).
- CAR-06 [MANDATORY]: Dispose owned resources deterministically with `using` or `await using`.
- CAR-07 [MANDATORY]: Never dispose dependencies resolved from the DI container; disposal is owned by the container lifecycle.
- CAR-08: Use `Span<T>` and `ReadOnlySpan<T>` only in measured hot paths where they clearly reduce allocations without harming maintainability.
- CDB-01 [MANDATORY]: Define and consume abstractions (interfaces or equivalent contracts) at architectural boundaries where substitution or testability is required.
- CDB-02 [MANDATORY]: Use constructor injection for required dependencies; do not hide required dependencies behind property injection.
- CDB-03 [MANDATORY]: Choose DI lifetimes deliberately (for example, singleton for stateless shared services, scoped for request-bound services, transient for lightweight stateless services) and keep lifetime usage valid.
- CDB-04 [MANDATORY]: Do not instantiate service dependencies with `new` inside business logic when DI should provide them.
- CDB-05 [MANDATORY]: Use the .NET Options pattern (`IOptions<T>`, `IOptionsSnapshot<T>`, or `IOptionsMonitor<T>`) and bind configuration into typed options classes.
- CDB-06 [MANDATORY]: Define explicit serialization DTOs and keep them separate from behavior-rich domain types.
- CDM-01: Default to immutable models by using `init` setters, `readonly` fields, and immutable collections unless mutability is required.
- CDM-02: Implement value objects as `record` or `record struct` when value-based equality is the intended behavior.
- CDM-03: Use `struct` only for small, immutable value types when profiling shows a measurable allocation or locality benefit.
- CDM-04: Keep each class focused on one responsibility; split classes when behavior changes for unrelated reasons.
- CDM-05: Write methods to perform one operation at one abstraction level; extract helper methods when a method mixes high-level flow and low-level detail.
- CDM-06: Use generic abstractions to share type-safe behavior instead of duplicating equivalent logic per type.
- CDM-07: Use pattern matching (`is`, `switch`, `switch` expressions) as the default for type and state branching when it improves clarity and exhaustiveness.
- CDM-08: Place extension members in focused static classes and use them only for cohesive behavior that is naturally discoverable on the extended type.
- CDM-09: Do not call `DateTime.UtcNow` or `DateTime.Now` directly in domain logic; inject time via `TimeProvider` or an equivalent abstraction.
- CDM-10: Do not instantiate randomness directly in domain logic (for example, `new Random()`); inject a randomness abstraction where determinism matters.
- CDM-11 [MANDATORY]: Treat shared mutable state as unsafe by default and require an explicit concurrency strategy (for example, immutability, locks, or concurrent collections).
- CDM-12: Keep framework-specific code (for example, ASP.NET Core and EF Core) at boundary layers and keep core domain logic framework-agnostic.
- CEL-01 [MANDATORY]: Validate method arguments at the boundary with guard clauses (for example, `ArgumentNullException.ThrowIfNull`) and fail fast on invalid inputs.
- CEL-02 [MANDATORY]: Throw exceptions only for exceptional conditions; use explicit return results for expected control flow outcomes.
- CEL-03 [MANDATORY]: Preserve diagnostic context when propagating exceptions: use `throw;` to rethrow and include the original exception as `InnerException` when wrapping.
- CEL-04 [MANDATORY]: Use structured logging with message templates and named properties (for example, `logger.LogInformation("Processed {OrderId}", orderId)`).
- CEL-05 [MANDATORY]: Never log secrets, credentials, tokens, or other sensitive values; log redacted or hashed identifiers where traceability is required.
- CEL-06 [MANDATORY]: Write comments only for intent, invariants, and non-obvious decisions; do not restate what the code already expresses clearly.
- CEL-07 [MANDATORY]: Document public APIs with XML documentation when intent, contracts, or failure behavior are not obvious from the signature.
- CEL-08 [MANDATORY]: Keep source-generated and hand-written code clearly separated (for example, generated files under a dedicated generated path, with partial types used only at intentional boundaries).
- CEL-09 [MANDATORY]: Do work that may fail off to the side before changing committed state. First prepare all risky work using temporary state; then commit using steps that cannot fail or are tightly controlled.
- CEL-10 [MANDATORY]: Prefer commit-or-rollback behavior when practical. A failed operation should ideally leave the observable state exactly as it was before the operation began
- CFT-01 [MANDATORY]: Set `TargetFramework` to `net10.0` in every production `.csproj` unless a documented compatibility constraint requires a different target.
- CFT-02 [MANDATORY]: Enable nullable reference types in every C# project by setting `<Nullable>enable</Nullable>` in the project file.
- CFT-03 [MANDATORY]: Set `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` and allow suppression only for explicitly documented warning IDs.
- CFT-04 [MANDATORY]: Enable .NET analyzers in all projects and define analyzer severity centrally in a source-controlled root `.editorconfig`.
- CFT-05 [MANDATORY]: Define repository-wide C# style in a root `.editorconfig` and enforce it in CI (for example, `dotnet format --verify-no-changes`).
- CFT-06 [MANDATORY]: Apply consistent naming conventions through `.editorconfig` naming rules and use intention-revealing names for types, members, parameters, and locals.
- CFT-07 [MANDATORY]: Express nullability intent explicitly in API signatures using nullable annotations (for example, `string?`) and related nullability attributes where needed.
- CFT-08 [MANDATORY]: Do not suppress nullable warnings (for example, with `!` or pragma directives) unless the exact reason is documented at the call site.
- CFT-09 [MANDATORY]: Default to the narrowest visibility (`private` or `internal`) and widen to `public` only when a real consumer requires it.
## Testing
- PBT-01 [MANDATORY]: The standard property based testing (PBT) stack is:
  - `FsCheck` for property based testing
  - `FsCheck.XUnit` for property based testing integration into xunit
- PBT-03 [MANDATORY]: Property Based Tests should be the default approach for testing that a SUT is broadly correct,
- PBT-04 [MANDATORY]: Unit tests should be reserved for regression cases, to test a specific case that is known to have previously caused issues.
- PBT-05 [MANDATORY]: Never just test single-point scenarios and  happy paths, instead use a Property Based Tests that will test all positive, negative and edge cases.
- PBT-06 [MANDATORY]: Define properties as universal rules that must hold for all valid inputs, rather than relying on specific example cases.
- PBT-07 [MANDATORY]: Design generators to produce diverse, realistic, and edge-case inputs across the full input space.
- PBT-08 [MANDATORY]: Ensure failing cases can be minimized automatically through shrinking to aid debugging.
- PBT-09 [MANDATORY]: Specify preconditions clearly or constrain generators so properties are only evaluated in valid domains.
- PBT-10 [MANDATORY]: Keep tests deterministic and reproducible by controlling randomness and eliminating hidden state or side effects.
- PBT-11 [MANDATORY]: Use strong oracles, models, or metamorphic relationships to validate correctness beyond simple assertions.

  - Every property must have an oracle: a mechanical way to decide pass/fail that is stronger than “doesn’t throw” or “looks plausible”.
  - Prefer a reference (spec) model oracle when you can: compute expected behaviour using a simpler, obviously-correct implementation and compare.
  - If you can’t compute the exact expected output, use a metamorphic oracle: apply a transformation to inputs and assert a predictable relationship between outputs.
  - Use multiple weak oracles together (invariants + metamorphic + cross-check) rather than one weak check.
  - Fail with evidence: when a property fails, ensure the counterexample is informative (shrinks well; includes classification/labels).
- PBT-12 [MANDATORY]: When making significant changes to a pre-existing unit test, convert it to a Property based test that tests a whole class of invariants and pre and post conditions. 
- TDD-001 [MANDATORY]: You MUST practice test-first development.  Follow the process of "Red-Green-Refactor"

  The Rules of TDD are:
  - Start with a PBT test that fails.
  - Make the smallest change needed to make that test pass.
  - Keep each step tiny so you focus on one thing at a time.

  Never get a failing test to pass by masking its failure.  Only a valid addition of functionality counts.
- TDD-002 [MANDATORY]: Test code should be developed first, NEVER in retrospect.
- TDD-003 [MANDATORY]: Observe a test failing first, before implementing the application code that makes it pass.
- TDD-004 [MANDATORY]: When a test finally passes, refactor the new code to ensure it is clean and has no technical debt.
- UTR-002: Avoid testing internal implementation details and avoid depending on concrete implementations where looser behavioral validation is possible.
- UTR-003: Never mask failing tests with broad `try` or `catch` blocks or "success assertions".
- UTR-004: Failing tests indentify outstanding work and should never be suppressed
## Performance
- PERF-01 [MANDATORY]: Measure before and after every non-trivial performance optimization.
- PERF-02 [MANDATORY]: Optimize only code paths that are proven hot or materially user-visible.
- PERF-03 [MANDATORY]: Never trade away correctness, determinism, or observability for performance.
- PERF-04 [MANDATORY]: Avoid unnecessary allocations in hot paths.
- PERF-05 [MANDATORY]: Avoid hidden boxing in hot paths.
- PERF-06 [MANDATORY]: Do not use allocation-heavy LINQ or iterator chains in hot paths when a simpler loop is materially cheaper.
- PERF-07 [MANDATORY]: Minimize transient string creation in hot paths.
- PERF-08 [MANDATORY]: Use span-based parsing and formatting APIs where they materially reduce copying or allocation.
- PERF-09: Use Span<T> and ReadOnlySpan<T> only where they improve performance without making ownership or lifetime unsafe or unclear.
- PERF-10: Use Memory<T> and ReadOnlyMemory<T> only where buffer lifetime must cross async or heap boundaries.
- PERF-11 [MANDATORY]: Avoid copying large buffers or collections when a safe slice, view, or reference is sufficient.
- PERF-12: Use stackalloc only for small, bounded, short-lived buffers.
- PERF-13: Use ArrayPool<T> when frequent buffer allocation creates measurable GC pressure.
- PERF-14: Use object pooling only for objects that are expensive to create or reset and are used predictably at high frequency.
- PERF-15 [MANDATORY]: Always return rented or pooled resources promptly and exactly once.
- PERF-16 [MANDATORY]: Do not expose pooled buffers or objects beyond the lifetime in which they are valid to use.
- PERF-17 [MANDATORY]: Use async and await for naturally asynchronous I/O-bound operations.
- PERF-18 [MANDATORY]: Do not block threads on asynchronous work in throughput-sensitive code paths.
- PERF-19: Use ValueTask only when measurement shows that avoiding Task allocation is materially beneficial.
- PERF-20 [MANDATORY]: Propagate CancellationToken in long-running or potentially blocking operations to avoid wasted work.
- PERF-21 [MANDATORY]: Use structured logging and avoid expensive log message construction when the log level is disabled.
- PERF-22 [MANDATORY]: Do not use exceptions for normal control flow in hot paths.
- PERF-23 [MANDATORY]: Validate arguments and fail early before expensive work begins.
- PERF-24 [MANDATORY]: Choose collection types based on required lookup, iteration, mutation, and allocation characteristics.
- PERF-25 [MANDATORY]: Avoid multiple enumeration of the same sequence in hot paths.
- PERF-26 [MANDATORY]: Prefer contiguous data access patterns when they materially improve cache locality.
- PERF-27: Use structs only when value semantics and measured allocation or locality benefits justify them.
- PERF-28: Use readonly struct for immutable value types that are frequently copied or passed by reference.
- PERF-29 [MANDATORY]: Prefer immutability by default, but avoid defensive copying in hot paths unless correctness requires it.
- PERF-30 [MANDATORY]: Prefer generic, type-safe code over object-based abstractions when object-based code would box or allocate materially more.
- PERF-31 [MANDATORY]: Avoid unnecessary virtual dispatch in hot paths when a simpler and equally maintainable alternative exists.
- PERF-32 [MANDATORY]: Avoid runtime reflection in hot paths.
- PERF-33: Prefer source generation over runtime reflection or runtime code discovery where it materially improves startup or throughput.
- PERF-34 [MANDATORY]: Choose serialization and deserialization paths that minimize allocation, copying, and intermediate materialization.
- PERF-35 [MANDATORY]: Stream or pipe large payloads instead of fully materializing them when full buffering is unnecessary.
- PERF-36 [MANDATORY]: Design hot-path code to reduce GC pressure rather than relying on forced collection.
- PERF-37 [MANDATORY]: Do not call GC.Collect in production code as a performance strategy.
- PERF-38 [MANDATORY]: Do not introduce parallelism unless the workload is safe, partitionable, and measurably faster under realistic contention.
- PERF-39 [MANDATORY]: Avoid shared mutable state and lock contention in throughput-sensitive code.
- PERF-40: Use SIMD, hardware intrinsics, or vectorized APIs only when measurement shows a clear benefit and portability remains acceptable.
- PERF-41 [MANDATORY]: Keep hot loops simple, branch-light, and allocation-free where practical.
- PERF-42 [MANDATORY]: Keep hot-path assumptions explicit and validate them outside the hot loop where possible.
- PERF-43: Change tiered compilation, quick JIT, or compilation settings only when benchmark evidence justifies it.
- PERF-44: Make libraries trimming-compatible when they are intended for trimmed deployments.
- PERF-45: Use Native AOT only when startup time, memory footprint, deployment model, or scale characteristics justify its constraints.
- PERF-46: Avoid dynamic code paths that prevent trimming or Native AOT where those deployment modes are required.
- PERF-47 [MANDATORY]: Use profiling and production-safe telemetry to locate CPU, memory, allocation, and latency bottlenecks.
- PERF-48 [MANDATORY]: Protect important performance characteristics with repeatable benchmarks or regression checks.
- PERF-49 [MANDATORY]: Design APIs so efficient usage is the default and expensive usage is explicit.
- PERF-50 [MANDATORY]: Prefer the simplest implementation that meets measured performance goals.
## Overview
- PLT-001: Plotter is a .NET tool for managing metadata about the plot of a novel.
## Completion
- UTR-001: A feature is not complete until integration tests prove it:

  1. Runs as intended in situ
  2. Executes successfully at runtime rather than merely compiling
  3. Produces results that are accessible and correct
  4. Exercises the major code paths and result types involved

  Features with only compilation tests or with failing runtime tests are incomplete.
