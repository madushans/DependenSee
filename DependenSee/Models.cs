namespace DependenSee;

public class DiscoveryResult
{
    public List<Project> Projects { get; } = new();
    public List<Package> Packages { get; } = new();
    public List<Reference> References { get; } = new();
}

public class Project
{
    public string Id { get; }
    public string Name { get; }

    public Project(string id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class Package
{
    public string Id { get; }
    public string Name { get; }

    public Package(string id, string name)
    {
        Id = id;
        Name = name;
    }
}

public class Reference
{
    public string From { get; }
    public string To { get; }

    public Reference(string from, string to)
    {
        From = from;
        To = to;
    }
}
