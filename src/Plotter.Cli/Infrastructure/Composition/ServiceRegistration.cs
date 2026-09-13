using Microsoft.Extensions.DependencyInjection;
using Plotter.Cli.Infrastructure.Configuration;
using Plotter.Cli.Infrastructure.Diagnostics;
using Plotter.Cli.Infrastructure.Storage;

namespace Plotter.Cli.Infrastructure.Composition;

public static class ServiceRegistration
{
  public static ServiceProvider Build()
  {
    var services = new ServiceCollection();
    services.AddSingleton<INovelFileResolver, NovelFileResolver>();
    services.AddSingleton<INovelWorkspaceStore, TomlWorkspaceStore>();
    services.AddSingleton<DiagnosticSink>();
    services.AddOptions<PlotterOptions>();
    return services.BuildServiceProvider();
  }
}
