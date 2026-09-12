using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Domain;
using Plotter.Cli.Presentation.Tui;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class InteractiveProjectionProperties
{
    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool SelectionStableAcrossTimelineAndManuscript(NovelWorkspace workspace)
    {
        var state = new TuiState();
        var timelineIds = TuiQueryAdapter.SceneIdsForView(workspace, "timeline", null, "story-time");
        var manuscriptIds = TuiQueryAdapter.SceneIdsForView(workspace, "manuscript", null, "story-time");
        var shared = timelineIds.Intersect(manuscriptIds, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
        if (shared is null)
            return true;
        state.Select(shared);
        state.ChangeView("timeline", timelineIds);
        state.ChangeView("manuscript", manuscriptIds);
        state.ChangeView("timeline", timelineIds);
        return state.SelectedSceneId == shared;
    }

    [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
    public bool ViewChangeWithoutSharedSceneFallsBack(NovelWorkspace workspace)
    {
        var state = new TuiState();
        var timelineIds = TuiQueryAdapter.SceneIdsForView(workspace, "timeline", null, "story-time");
        var manuscriptIds = TuiQueryAdapter.SceneIdsForView(workspace, "manuscript", null, "story-time");
        var onlyTimeline = timelineIds.Except(manuscriptIds, StringComparer.OrdinalIgnoreCase).FirstOrDefault();
        if (onlyTimeline is null)
            return true;
        state.Select(onlyTimeline);
        state.ChangeView("manuscript", manuscriptIds);
        return state.SelectedSceneId != onlyTimeline;
    }
}