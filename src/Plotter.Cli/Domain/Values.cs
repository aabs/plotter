namespace Plotter.Cli.Domain;

/// <summary>Elapsed-time classification used by gap reports.</summary>
public enum GapClass
{
    Normal,
    Long,
    Overnight,
    Unknown,
}

/// <summary>Severity of an audit or validation finding.</summary>
public enum FindingSeverity
{
    Error,
    Warning,
    Info,
}

/// <summary>Writer-assigned relationship of a scene to a plot thread.</summary>
public enum PlotThreadClassification
{
    Primary,
    Secondary,
    Absent,
    NotClassified,
}

/// <summary>Certainty of a story date/time attribution.</summary>
public enum DateCertainty
{
    Known,
    Approximate,
    Unknown,
}

/// <summary>A non-negative story duration for a scene.</summary>
public readonly record struct SceneDuration
{
    public TimeSpan Value { get; }

    public SceneDuration(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
            throw new ArgumentException("Scene duration cannot be negative.", nameof(value));
        Value = value;
    }
}

/// <summary>An in-world date/time attribution with explicit certainty.</summary>
public sealed record StoryTime(DateTime? Date, DateCertainty Certainty = DateCertainty.Known);

/// <summary>An explicit writer-provided qualification used by audits (for example time-confidence).</summary>
public sealed record ContinuityAnnotation(string Name, string Value)
{
    public static ContinuityAnnotation TimeConfidence(string value) => new("time-confidence", value);

    public static ContinuityAnnotation TravelStatus(string value) => new("travel-status", value);

    public static ContinuityAnnotation ContinuityStatus(string value) => new("continuity-status", value);
}
