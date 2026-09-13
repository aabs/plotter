namespace Plotter.Cli.Domain;

public readonly record struct SceneId
{
  public string Value { get; }

  public SceneId(string value) => Value = IdRules.Required(value, nameof(SceneId));

  public override string ToString() => Value;
}

public readonly record struct ParticipantId
{
  public string Value { get; }

  public ParticipantId(string value) => Value = IdRules.Required(value, nameof(ParticipantId));

  public override string ToString() => Value;
}

public readonly record struct LocationId
{
  public string Value { get; }

  public LocationId(string value) => Value = IdRules.Required(value, nameof(LocationId));

  public override string ToString() => Value;
}

public readonly record struct PlotId
{
  public string Value { get; }

  public PlotId(string value) => Value = IdRules.Required(value, nameof(PlotId));

  public override string ToString() => Value;
}

public readonly record struct GroupId
{
  public string Value { get; }

  public GroupId(string value) => Value = IdRules.Required(value, nameof(GroupId));

  public override string ToString() => Value;
}

internal static class IdRules
{
  public static string Required(string value, string label)
  {
    if (string.IsNullOrWhiteSpace(value))
      throw new ArgumentException($"{label} must be non-empty.", nameof(value));
    return value.Trim();
  }
}
