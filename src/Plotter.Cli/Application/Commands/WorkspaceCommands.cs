using Plotter.Cli.Domain;
using Plotter.Cli.Infrastructure.Storage;

namespace Plotter.Cli.Application.Commands;

public sealed class WorkspaceCommands(INovelWorkspaceStore store)
{
  public async Task<CommandResult> InitAsync(string file, CancellationToken cancellationToken = default)
  {
    await store.SaveAsync(file, new NovelWorkspace(), cancellationToken);
    return new CommandResult(true);
  }

  public async Task<NovelWorkspace> OpenAsync(string file, CancellationToken cancellationToken = default) =>
      await store.LoadAsync(file, cancellationToken);

  public async Task<CommandResult> SaveAsync(string file, NovelWorkspace workspace, CancellationToken cancellationToken = default)
  {
    await store.SaveAsync(file, workspace, cancellationToken);
    return new CommandResult(true);
  }
}
