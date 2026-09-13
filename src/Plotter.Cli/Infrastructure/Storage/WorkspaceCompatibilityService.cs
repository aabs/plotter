using Tomlyn;

namespace Plotter.Cli.Infrastructure.Storage;

public static class WorkspaceCompatibilityService
{
  public const string CurrentFormatVersion = "1";

  public static bool IsCompatibleVersion(string? version) =>
      version is null || version == CurrentFormatVersion;

  /// <summary>Returns a compatibility/validity message, or null when the file is compatible.</summary>
  public static string? Validate(string path)
  {
    if (!File.Exists(path))
      return null;
    try
    {
      var text = File.ReadAllText(path);
      var model = Toml.ToModel(text);
      var version = model.TryGetValue("format_version", out var value) ? value as string : null;
      return IsCompatibleVersion(version)
          ? null
          : $"Unsupported format version '{version}' (expected '{CurrentFormatVersion}').";
    }
    catch (Exception error)
    {
      return $"Malformed TOML in '{path}': {error.Message}";
    }
  }
}
