namespace Plotter.Cli.Infrastructure.Configuration;

public interface INovelFileResolver
{
    string Resolve(string? explicitPath);
}

public sealed class NovelFileResolver : INovelFileResolver
{
    public string Resolve(string? explicitPath)
    {
        if (!string.IsNullOrWhiteSpace(explicitPath))
            return Path.GetFullPath(explicitPath);
        return Path.Combine(Directory.GetCurrentDirectory(), "novel.toml");
    }
}