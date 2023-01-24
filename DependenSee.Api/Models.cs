namespace DependenSee.Api;

/// <summary>
/// Result of a discovery run
/// </summary>
public class DiscoveryResult
{
    /// <summary>
    /// Details of a discovered projects.
    /// </summary>
    public List<Project> Projects { get; } = new List<Project>();

    /// <summary>
    /// Details of a discovered packages that are referenced by discovered <see cref="Projects"/>.
    /// </summary>
    public List<Package> Packages { get; } = new List<Package>();

    /// <summary>
    /// Details of how projects and packages are referenced
    /// </summary>
    public List<Reference> References { get; } = new List<Reference>();
}


/// <param name="Id">
/// Unique identifier for this project. This is 
/// generated and serves only to uniquely identify this project. 
/// This is usually a partial/relative file path, however it is not
/// guaranteed to be so.
/// </param>
/// <param name="Name">
/// Name of the project, usually derived from the MSBuild filename.
/// </param>
public record class Project(string Id, string Name);


/// <param name="Id">
/// Unique identifier for this package. This is usually the 
/// package name, but is not guaranteed to be so.
/// </param>
/// <param name="Name">
/// Name of the package.
/// </param>
public record class Package(string Id, string Name);

/// <summary>
/// Description of a reference between a project and another
/// project or a package.
/// </summary>
/// <param name="From">
/// Reference origination. This is an <see cref="Project.Id"/>
/// of a <see cref="Project"/>
/// </param>
/// <param name="To">
/// Reference destination. This is an <see cref="Project.Id"/>
/// of a <see cref="Project"/> or an <see cref="Package.Id"/>
/// of a <see cref="Package"/>
/// </param>
public record class Reference(string From, string To);


