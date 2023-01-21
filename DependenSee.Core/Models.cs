namespace DependenSee.Core;

public class DiscoveryResult
{
    public List<Project> Projects { get; set; }
    public List<Package> Packages { get; set; }
    public List<Reference> References { get; set; }
}

public class Project
{
    public  string Id { get; set; }
    public string Name { get; set; }
}
public class Package
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Reference
{
    public string From { get; set; }
    public string To { get; set; }
}
