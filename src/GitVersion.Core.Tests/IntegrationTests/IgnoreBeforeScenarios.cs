using GitVersion.Configuration;
using GitVersion.Core.Tests.Helpers;

namespace GitVersion.Core.Tests.IntegrationTests;

// TODO 3074: add test cases for ignored commits
[TestFixture]
public class IgnoreBeforeScenarios : TestBase
{
    [TestCase(null, "0.0.1+0")]
    [TestCase("0.0.1", "0.0.1+0")]
    [TestCase("0.1.0", "0.1.0+0")]
    [TestCase("1.0.0", "1.0.0+0")]
    public void ShouldFallbackToBaseVersionWhenAllCommitsAreIgnored(string? nextVersion, string expectedFullSemVer)
    {
        using var fixture = new EmptyRepositoryFixture();
        var dateTimeNow = DateTimeOffset.Now;
        fixture.MakeACommit();

        var configuration = GitFlowConfigurationBuilder.New.WithNextVersion(nextVersion)
            .WithIgnoreConfiguration(new() { Before = dateTimeNow.AddDays(1) }).Build();

        fixture.AssertFullSemver(expectedFullSemVer, configuration);
    }

    [TestCase(null, "0.0.1+1")]
    [TestCase("0.0.1", "0.0.1+1")]
    [TestCase("0.1.0", "0.1.0+1")]
    [TestCase("1.0.0", "1.0.0+1")]
    public void ShouldNotFallbackToBaseVersionWhenAllCommitsAreNotIgnored(string? nextVersion, string expectedFullSemVer)
    {
        using var fixture = new EmptyRepositoryFixture();
        var dateTimeNow = DateTimeOffset.Now;
        fixture.MakeACommit();

        var configuration = GitFlowConfigurationBuilder.New.WithNextVersion(nextVersion)
            .WithIgnoreConfiguration(new() { Before = dateTimeNow.AddDays(-1) }).Build();

        fixture.AssertFullSemver(expectedFullSemVer, configuration);
    }

    [Test]
	// TODO: adaüt
    public void ShouldFallbackToBaseVersionWhenAllCommitsAreIgnored_IgnoreCommits()
    {
        using var fixture = new EmptyRepositoryFixture();
        fixture.Repository.MakeATaggedCommit("1.0.3");
        fixture.Repository.CreateBranch("develop");
        fixture.Repository.MakeCommits(1);
        var commit = fixture.Repository.MakeACommit("+semver:major");

        var config = new ConfigurationBuilder()
            .Add(new Config
            {
                Ignore = new IgnoreConfig
                {
                    ShAs = new[] { commit.Sha }
                }
            }).Build();

        fixture.AssertFullSemver("1.0.4+2", config);
    }


    [Test]
    public void ShouldFallbackToBaseVersionWhenAllMergeCommitsAreIgnored()
    {
        using var fixture = new EmptyRepositoryFixture();
        fixture.Repository.MakeATaggedCommit("1.0.3");
        fixture.Repository.CreateBranch("develop");
        fixture.Repository.MakeCommits(1);

        fixture.Repository.CreateBranch("feature/feature1");
        fixture.Checkout("feature/feature1");
        var commit = fixture.Repository.MakeACommit("+semver:major");
        fixture.Checkout("develop");
        fixture.Repository.MergeNoFF("feature/feature1", Generate.SignatureNow());

        var config = new ConfigurationBuilder()
            .Add(new Config
            {
                Ignore = new IgnoreConfig
                {
                    ShAs = new[] { commit.Sha }
                }
            }).Build();

        fixture.AssertFullSemver("1.1.0-alpha.3", config);
    }
}
