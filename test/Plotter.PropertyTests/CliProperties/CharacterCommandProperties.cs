using FsCheck;
using FsCheck.Xunit;
using Plotter.Cli.Application.Queries;
using Plotter.Cli.Domain;
using Plotter.PropertyTests.Generators;

namespace Plotter.PropertyTests.CliProperties;

public sealed class CharacterCommandProperties
{
  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool EmptyParticipantSelectionYieldsNoRows(NovelWorkspace workspace)
  {
    var result = CharacterContinuityQueries.GetLanes(workspace, []);
    return result.Rows.Count == 0;
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool GroupResolvesMemberParticipants(SceneId groupId, ParticipantId member)
  {
    var workspace = new NovelWorkspace();
    workspace.Participants[member.Value] = new Participant(member, member.Value);
    workspace.ParticipantGroups[groupId.Value] = new ParticipantGroup(new GroupId(groupId.Value), groupId.Value, [member]);
    var resolved = CharacterContinuityQueries.ResolveGroup(workspace, groupId.Value);
    return resolved.Contains(member.Value, StringComparer.OrdinalIgnoreCase);
  }

  [Property(Arbitrary = new[] { typeof(DomainGenerators) })]
  public bool UnknownGroupResolvesToNoParticipants(NovelWorkspace workspace, SceneId groupName)
  {
    var resolved = CharacterContinuityQueries.ResolveGroup(workspace, groupName.Value);
    return resolved.Count == 0;
  }
}
