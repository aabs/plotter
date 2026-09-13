using System.Text.Json;

namespace Plotter.Cli.Application.Projections;

public interface IResultProjection<in TResult>
{
  string ToText(TResult result);

  string ToJson(TResult result);

  string ToCsv(TResult result);
}
