namespace DependenSee.Api;

public class DiscoveryResult
{
    public List<Project> Projects { get; } = new List<Project>();
    public List<Package> Packages { get; } = new List<Package>();
    public List<Reference> References { get; } = new List<Reference>();
}

public record class Project(string Id, string Name);

public record class Package(string Id, string Name);

public record class Reference(string From, string To);


