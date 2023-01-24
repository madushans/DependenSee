namespace DependenSee.Api;

public record class DiscoveryRequest
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string SourceFolder { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string[]? ExcludeFolders { get; init; } = null;
    public bool FollowReparsePoints { get; init; } = false;
}
