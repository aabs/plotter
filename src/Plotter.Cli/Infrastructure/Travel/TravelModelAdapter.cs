namespace Plotter.Cli.Infrastructure.Travel;

public interface ITravelModelAdapter
{
    TimeSpan? Estimate(string fromLocationId, string toLocationId);
}

public sealed class NullTravelModel : ITravelModelAdapter
{
    public TimeSpan? Estimate(string fromLocationId, string toLocationId) => null;
}
