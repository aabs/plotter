using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Travel;
using Plotter.Cli.Presentation.Cli;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class TravelCommandProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool RenderNeverShowsInventedRouteDurations(NovelWorkspace workspace)
    {
        var participant = workspace.Participants.Keys.FirstOrDefault();
        if (participant is null)
            return true;
        var result = TravelQueries.GetTravel(workspace, participant, new NullTravelModel());
        var writer = new StringWriter();
        TravelRenderers.RenderTravel(workspace, result, writer);
        var output = writer.ToString();
        return !output.Contains('→');
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool RenderShowsAvailableTimeWhenPresent(NovelWorkspace workspace)
    {
        var participant = workspace.Participants.Keys.FirstOrDefault();
        if (participant is null)
            return true;
        var result = TravelQueries.GetTravel(workspace, participant, new NullTravelModel());
        if (!result.Steps.Any(step => step.AvailableUntilNext is not null))
            return true;
        var writer = new StringWriter();
        TravelRenderers.RenderTravel(workspace, result, writer);
        return writer.ToString().Contains("available", StringComparison.Ordinal);
    }
}