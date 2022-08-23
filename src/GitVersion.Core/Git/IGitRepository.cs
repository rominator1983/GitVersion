namespace GitVersion;

public interface IGitRepository : IDisposable
{
    string Path { get; }
    string WorkingDirectory { get; }
    bool IsHeadDetached { get; }
    IBranch Head { get; }
    ITagCollection Tags { get; }
    IReferenceCollection Refs { get; }
    IBranchCollection Branches { get; }
    IEnumerable<ICommit> Commits { get; }

    IEnumerable<ICommit> QueryBy(CommitFilter commitFilter);
    IRemoteCollection Remotes { get; }

    ICommit? FindMergeBase(ICommit commit, ICommit otherCommit);

    int GetNumberOfUncommittedChanges();
    void DiscoverRepository(string? gitDirectory);
}
