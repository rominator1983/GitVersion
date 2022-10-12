using GitVersion.Model.Configuration;

namespace GitVersion.VersionCalculation;

public interface IIncrementStrategyFinder
{
    VersionField DetermineIncrementedField(GitVersionContext context, BaseVersion baseVersion, EffectiveConfiguration configuration);

    VersionField? GetIncrementForCommits(Model.Configuration.GitVersionConfiguration configuration, IEnumerable<ICommit> commits);
}
