using GitVersion.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace GitVersion;

public class GitVersionLibGit2SharpModule : IGitVersionModule
{
    public void RegisterTypes(IServiceCollection services)
    {
        services.AddSingleton<IGitRepository>(sp =>
        {
            var repositoryInfo = sp.GetRequiredService<IGitRepositoryInfo>();
            var log = sp.GetRequiredService<ILog>();
            IGitRepository gitRepository = new GitRepository(log);
            gitRepository.DiscoverRepository(repositoryInfo.GitRootPath);
            return gitRepository;
        });

        services.AddSingleton<IIgnoredFilterProvider, IgnoredFilterProvider>();
        services.AddSingleton<IGitRepository, IgnoredFilteringGitRepositoryDecorator>();

        services.AddSingleton<IMutatingGitRepository, IgnoredFilteringGitRepositoryDecorator>();

        services.AddSingleton<IGitRepositoryInfo, GitRepositoryInfo>();
    }
}
