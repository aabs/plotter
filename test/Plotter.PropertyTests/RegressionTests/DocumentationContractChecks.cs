using FsCheck;
using FsCheck.Xunit;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.RegressionTests;

public sealed class DocumentationContractChecks
{
  private static string RepoRoot()
  {
    var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "plotter.slnx")))
      directory = directory.Parent;
    return directory?.FullName ?? Directory.GetCurrentDirectory();
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool DocumentedCommandsAreImplementedVocabulary()
  {
    var reference = File.ReadAllText(Path.Combine(RepoRoot(), "docs", "command-reference.md"));
    string[] expected =
    [
        "novel init", "novel participant", "novel scene", "novel timeline", "novel character",
            "novel location", "novel locations", "novel thread", "novel threads", "novel audit",
            "novel continuity", "novel travel", "novel calendar", "novel export", "novel lanes",
            "novel where", "novel plot", "novel interaction", "novel tui",
        ];
    return expected.All(command => reference.Contains(command, StringComparison.Ordinal));
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool GettingStartedCommandsAreDocumented()
  {
    var reference = File.ReadAllText(Path.Combine(RepoRoot(), "docs", "getting-started.md"));
    return reference.Contains("novel init", StringComparison.Ordinal)
        && reference.Contains("novel scene set", StringComparison.Ordinal)
        && reference.Contains("novel timeline", StringComparison.Ordinal);
  }
}
