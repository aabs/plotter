namespace Plotter.Cli.Infrastructure.Configuration;

public sealed class NovelFileResolver
{
    public static string Resolve(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
            return Path.GetFullPath(explicitPath);
        return Path.Combine(Directory.GetCurrentDirectory(), "novel.toml");
    }
}
