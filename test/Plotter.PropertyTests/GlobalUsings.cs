global using Xunit;

// Command-module tests temporarily redirect Console.Out; disable parallelization
// so the process-global console redirection cannot interfere across test classes.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
